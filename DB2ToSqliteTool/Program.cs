using DB2ToSqliteTool.Helpers;
using DB2ToSqliteTool.Services;
using DBCD;
using DBCD.IO;
using DBCD.Providers;
using DBDefsLib;
using Microsoft.Data.Sqlite;
using TACTSharp;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false)
    .Build();
var settingsz = configuration.GetSection("Settings").Get<object>();
var settings = configuration.GetSection("Settings").Get<BindableSettings>();
if (settings == null) throw new Exception("Failed to load Settings from configuration.");

var listFile = new Listfile();

listFile.Initialize(new CDN(settings), settings);

var buildInfo = new BuildInfo(Path.Combine(settings.BaseDir, ".build.info"), settings, new CDN(settings));

var buildInstance = new BuildInstance();
var entry = buildInfo.Entries.First(x => x.Product == settings.Product);

buildInstance.Settings.BuildConfig ??= entry.BuildConfig;
buildInstance.Settings.CDNConfig ??= entry.CDNConfig;

buildInstance.LoadConfigs(buildInstance.Settings.BuildConfig, buildInstance.Settings.CDNConfig);
buildInstance.Load();

var tables = configuration.GetSection("Tables").Get<IEnumerable<string>>();

var targetDirectory = configuration.GetValue<string>("TargetDirectory") ?? "dbfilesclient";
var databaseFile = configuration.GetValue<string>("DatabaseFile") ?? "wowsims.db";

Directory.CreateDirectory(targetDirectory);

var fsProvider = new FilesystemDBCProvider(targetDirectory, true);
var githubDbdProvider = new GithubDBDProvider(true);

var dbcd = new DBCD.DBCD(fsProvider, githubDbdProvider);

var dbDefinitions = new Dictionary<string, Structs.DBDefinition>();
var storageMap = new Dictionary<string, IDBCDStorage>();

if (tables != null)
    foreach (var tableName in tables)
    {
        var file = buildInstance.OpenFileByFDID(listFile.GetFDID($"{targetDirectory}/{tableName}.db2"));
        await File.WriteAllBytesAsync($"{targetDirectory}/{tableName}.db2", file);

        var tableDefStream = githubDbdProvider.StreamForTableName(tableName, entry.Version);
        var dbReader = new DBDReader();
        var tableDefinition = dbReader.Read(tableDefStream, true);

        var storage = dbcd.Load(tableName, entry.Version);

        dbDefinitions.Add(tableName, tableDefinition);
        storageMap.Add(tableName, storage);
    }

var splitBuild = entry.Version.Split('.');
if (splitBuild.Length != 4)
    throw new Exception("Invalid build!");

var buildNumber = uint.Parse(splitBuild[3]);
SqliteDbCreator.CreateDatabaseWithDefinitions(dbDefinitions, databaseFile, buildNumber);


if (HotfixManager.hotfixReaders.Count == 0)
    HotfixManager.LoadCaches();
if (!HotfixManager.hotfixReaders.TryGetValue(buildNumber, out var hotfixReader))
    throw new Exception("No hotfix found for build " + buildNumber);

var connectionString = new SqliteConnectionStringBuilder
{
    DataSource = databaseFile,
    Mode = SqliteOpenMode.ReadWriteCreate
}.ToString();

await using var conn = new SqliteConnection(connectionString);

conn.Open();

foreach (var tableName in tables)
{
    var storage = storageMap[tableName];
    storage.ApplyingHotfixes(hotfixReader);
    SqliteDataInserter.InsertRows(storage, tableName, dbDefinitions[tableName], conn, buildNumber);
}

conn.Close();

Console.WriteLine("Processing completed.");