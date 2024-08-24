# FF16Tools

Tools for Final Fantasy XVI / 16. 

### Features

* `.pac` Unpacker/Repacker
* `.tex` to `.png` conversion

This has been tested on Final Fantasy 16 Demo (Steam/PC) files.

## Usage

Get the latest version in [**Releases**](https://github.com/Nenkai/FF16Pack/releases).
* Unpacking all files: `FF16Tools.CLI unpack-all -i <path_to_pac> [-o output_directory]`
* Unpacking a specific file: `FF16Tools.CLI unpack -i <path_to_pac> -f <game_file> [-o output_directory]`
* Pack a directory into a `.pac`: `FF16Tools.CLI pack -i <path_to_directory> [-o output_directory]`
* Listing files: `FF16Tools.CLI list-files -i <path_to_pac>`
* Converting `.tex` to `.png`: Drag-drop files/folders or `FF16Tools.CLI tex-conv -i <path>`

### Modding

> [!NOTE]  
> You may choose to rebuild a `.pac` entirely, or **preferably** you can use `.diff.pac` files.
> 
> If you wish to edit files already present in pac files, copy their file structure into a new directory, then pack it. Rename the result `.pac` file `<name>.diff.pac`.
> 
> So if your files came from `0000.pac`, the new pac file should be `0000.diff.pac`. 

> [!IMPORTANT]
> **You only have to include files you want to edit, not ALL the files from the original `.pac`**.
> You might also have to set the internal archive name/directory using `--name`, if the original had it.

## Building

Requires **.NET 8.0** (VS2022), Windows, DirectStorage support.

## Format Documentation

* [010 Editor Templates](https://github.com/Nenkai/010GameTemplates/tree/main/Square%20Enix/Final%20Fantasy%2016)
* [C# Classes](https://github.com/Nenkai/FF16Pack/tree/master/FF16PackLib)

## Roadmap

* Return building/unpack progress
* Expose lib as a NuGet Package (for package consumption from i.e mod managers)
* Find a GDeflate library that actually works cross-platform

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
