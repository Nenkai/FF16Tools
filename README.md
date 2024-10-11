# FF16Tools

Tools for Final Fantasy XVI / 16. 

### Features

* `.pac` Unpacker/Repacker
* `.tex` to `.dds` conversion
* `.dds` & other image formats to `.tex`
* `.nxd`  (Nex/Next ExcelDB) conversion
* Save file (`.png`) unpack/pack (from: `Documents\My Games\FINAL FANTASY XVI\Steam\<steam_id>`)

This has been tested on Final Fantasy 16 (Steam/PC) files.

## Usage

Get the latest version in [**Releases**](https://github.com/Nenkai/FF16Pack/releases).

### Pac File
You can use the **GUI/Graphical version**, or for the command line (CLI) commands:

* Unpacking all files from a pack: `FF16Tools.CLI unpack-all -i <path_to_pac> [-o <output_directory>]`
* Unpacking a specific file from a specific pack: `FF16Tools.CLI unpack -i <path_to_pac> -f <game_file> [-o <output_directory>]`
* Unpacking all packs: `FF16Tools.CLI unpack-all-packs -i <path_to_folder_with_packs> [-o <output_directory>]`
* Pack a directory into a `.pac`: `FF16Tools.CLI pack -i <path_to_directory> [-o <output_directory>]`
* Listing files: `FF16Tools.CLI list-files -i <path_to_pac>`

### Textures
* Converting `.tex` to `.dds`: Drag-drop files/folders or `FF16Tools.CLI tex-conv -i <path>`
* Converting to `.tex`: Drag-drop files or `FF16Tools.CLI img-conv -i <path>`

> [!WARNING]  
> * 3D textures are not yet supported.

### Nex (NXD)

* To SQLite: `nxd-to-sqlite -i <path to directory> [-o output sqlite file]`
* From SQLite: `sqlite-to-nxd -i <path to sqlite file> [-t <table_list_separated_by_spaces> -o <output_directory>]`

> [!NOTE]
> * When converting from SQLite to Nex, you can provide a table list to avoid converting all the tables if not needed.
> * Use a SQLite database editor/viewer such as [SQLiteStudio](https://sqlitestudio.pl/).
> * Always check the [Changelog](NEX_CHANGELOG.md) for updated table column names.
> * Refer to the [table layouts here](FF16Tools.Files/Nex/Layouts) for the column value types. Note: this has been mapped mostly manually. Please contribute if something is amiss or you have figured out the column names.
> * Nex can contain nested data, therefore arrays and other structs are converted to json strings.
> * Nex can contain row sets that don't actually contain any rows. This information is lost between SQLite conversion, but should *hopefully* not matter.
> * You may need to edit `root.nxl` from `0001` to reflect the number of rows (if you've added/removed any).

## Modding

### Method 1: Automatic Mod Loader

Use [Reloaded-II](https://github.com/Reloaded-Project/Reloaded-II/releases) and install the [FFXVI Mod Loader](https://github.com/Nenkai/ff16.utility.modloader).

Refer to [this](https://nenkai.github.io/ffxvi-modding/modding/installing_mods/).

### Method 2: Modding Manually

> [!NOTE]
> You should use **Method 1** if you intend to distribute mods, otherwise it is not possible for users to install multiple mods that edits the same pack contents.

You may choose to rebuild a `.pac` entirely, or **preferably** you can use `.diff.pac` files.

When packing an extracted folder, the output pack file should have `.diff` in its name. The game will load this file.

So if your files came from `0000.pac`, the new pac file should be `0000.diff.pac`. 

> [!IMPORTANT]
> **You only have to include files you want to edit, not ALL the files from the original `.pac`**.
> 
> If the extracted archive had a `.path` file, it should be present when packing.
> 
> If you are editing any of the language files (like `0001.en.pac`), you should have `0001.diff.pac` AND `0001.diff.en.pac`. if you don't have the first one, the second one won't load. If you don't need to edit the first one, just copy it and rename it.

#### Extra Notes

Each pack file can contain a general embedded folder name. This is the case for `0001.pac`, or nested packs inside `0000.pac`.

For instance, `0001.pac` has `nxd` has its embedded folder name. Which means that every file in that pack is inside a folder named `nxd`. Example: `ability.nxd` becomes `nxd/ability.nxd` 

This can be tricky to handle, so the *unpacker* creates a `.path` file with the name of the folder.

The *packer* picks the path from this file accordingly. It can also be set manually with the `--name` argument, if needed.

## NuGet
* [FF16Tools.Pack](https://www.nuget.org/packages/FF16Tools.Pack)
* [FF16Tools.Files](https://www.nuget.org/packages/FF16Tools.Files/)

## Building

Requires **.NET 8.0** (VS2022), Windows, DirectStorage support.

## Format Documentation

* [010 Editor Templates](https://github.com/Nenkai/010GameTemplates/tree/main/Square%20Enix/Final%20Fantasy%2016)
* [C# Classes](https://github.com/Nenkai/FF16Pack/tree/master/FF16PackLib)

## Roadmap

* Return building/unpack progress
* Find a GDeflate library that actually works cross-platform

## Discord

<a href="https://discord.gg/D7jhUDfYZh">
  <img src="https://discordapp.com/api/guilds/1284918645675397140/widget.png?style=banner2" alt="Discord Banner 1"/>
</a>

## Acknowledgements

[Vortice.Windows](https://github.com/amerkoleci/Vortice.Windows) for having a usable DirectStorage wrapper/API.
* [GisDeflate](https://github.com/sk-zk/GisDeflate) would break on a certain file (also doesn't have span interfaces),
* [Silk.NET](https://github.com/dotnet/Silk.NET) has 0 documentation, and would return obtuse windows errors such as `0x8004001` (not implemented). Very useful!

---
* [Nenkai](https://github.com/Nenkai) - Reverse-Engineering, Unpack/Packing
* [frostbone25](https://github.com/frostbone25) GUI Version of the Unpack tool (thanks!)
* [yretenai](https://github.com/yretenai) - Information on the file structure

## License

MIT License. Make sure to credit if you reuse this!
