# DB2ToSqliteTool

DB2ToSqliteTool is a C# CLI tool that parses World of Warcraft client tables and exports them into a SQLite database.

## Configuration

The tool is configured via an `appsettings.json` file. The following settings are required:

```json
{
"Settings": {
    "BaseDir": "F:\\World of Warcraft",
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
    - **Region**: The region of your client (e.g., "us").
    - **Product**: The WoW product (e.g., "wow_classic"). A list of products can be found at [wago.tools](https://wago.tools/).

- **TargetDirectory**: The directory where the .db2 files will be temporarily stored.
- **DatabaseFile**: The output file path for the generated SQLite database.
- **Tables**: An array of table names to include. Only tables listed here will be processed.

## Usage

### 1. Download Submodules

Make sure to initialize and update submodules before building:

```bash
git submodule update --init --recursive
dotnet build
dotnet run --project DB2ToSqliteTool
```

Alternatively, you can run the produced executable after building.

### 4. Hotfixes

If you want to apply hotfixes, run the game client first to ensure that the latest hotfixes are available.

## Credits

- **[wow.tools.local](https://github.com/Marlamin/wow.tools.local)** – Thanks for the inspiration and components `DBCacheParser` and `HotfixManager` among others.
- **[TACTSharp](https://github.com/wowdev/TACTSharp.git)** – Library for reading WoW client files.
- **[DBCD](https://github.com/wowdev/DBCD.git)** – Library for processing DB2 definitions and data.

## License

This project is licensed under the MIT License.
