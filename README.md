# DB2ToSqliteTool

DB2ToSqliteTool is a C# CLI tool that parses World of Warcraft client tables and exports them into a SQLite database. It leverages the [TACTSharp](https://github.com/wowdev/TACTSharp.git) and [DBCD](https://github.com/wowdev/DBCD.git) libraries to read and process WoW client .db2 files, and supports applying hotfixes if your client is up-to-date.

## Features

- **Parse WoW Client Tables**: Extracts .db2 table data from your WoW client.
- **SQLite Export**: Converts the parsed data into a configurable SQLite database.
- **Configurable via JSON**: All key parameters are set in `appsettings.json`.
- **Hotfix Support**: Optionally apply hotfixes (ensure you run the game to update your client hotfixes).

## Configuration

The tool is fully configured via an `appsettings.json` file. The following settings are required:

```json
{
"Settings": {
"BaseDir": "F:\\\\World of Warcraft",
"BuildConfig": "buildConfig",
"CDNConfig": "cdnConfig",
"Region": "us",
"Product": "wow_classic"
},
"TargetDirectory": "dbfilesclient",
"DatabaseFile": "wowsims.db",
"Tables": [
"Spell",
"Curve",
"CurvePoint",
"Difficulty",
"SpellRadius",
"SpellClassOptions",
"SpellEffect"
]
}
```

- **Settings**
    - **BaseDir**: The root directory of your World of Warcraft client.
    - **BuildConfig**: Your client's build configuration.
    - **CDNConfig**: Your client's CDN configuration.
    - **Region**: The region of your client (e.g., "us").
    - **Product**: The WoW product (e.g., "wow_classic"). A list of valid products can be found at [wago.tools](https://wago.tools/).

- **TargetDirectory**: The directory where the .db2 files will be temporarily stored.
- **DatabaseFile**: The output file path for the generated SQLite database.
- **Tables**: An array of table names to include. Only tables listed here will be processed.

## Usage

### 1. Download Submodules

Make sure to initialize and update submodules before building:

```bash
git submodule update --init --recursive
```

### 2. Build the Project

Use the .NET CLI to build the project:

```bash
dotnet build
```

### 3. Run the Tool

Run the tool using the .NET CLI:

```bash
dotnet run --project DB2ToSqliteTool
```

Alternatively, you can run the produced executable after building.

### 4. Hotfixes

If you want to apply hotfixes, run the game client first to ensure that the latest hotfixes are available.

The SQLite database will be generated at the path specified by the \`DatabaseFile\` setting.

## How It Works

1. **Initialization**:  
   The tool reads settings and table names from \`appsettings.json\` and initializes TACTSharp components along with the build instance.

2. **Table Processing**:  
   For each table listed in the configuration, the tool:
    - Extracts the corresponding .db2 file and writes it to the target directory.
    - Retrieves and parses the table definition.
    - Loads the table storage and saves both the definition and storage in memory.

3. **Database Creation & Insertion**:  
   After processing all tables, the tool creates the SQLite database using the collected definitions. It then applies hotfixes (if available) and inserts all table data into the database.

## Credits

- **[wow.tools.local](https://github.com/Marlamin/wow.tools.local)** – Thanks for the inspiration and components.
- **[TACTSharp](https://github.com/wowdev/TACTSharp.git)** – Library for reading WoW client files.
- **[DBCD](https://github.com/wowdev/DBCD.git)** – Library for processing DB2 definitions and data.

## License

This project is licensed under the MIT License.
