# FF16Pack

Unpacker for Final Fantasy XVI / 16 .pac files.

This has been tested on Final Fantasy 16 Demo (Steam/PC) files.

## Usage

Get the latest version in [**Releases**](https://github.com/Nenkai/FF16Pack/releases).
* Unpacking all files: `FF16Pack.CLI unpack-all -i <path_to_pac> [-o output_directory]`
* Unpacking a specific file: `FF16Pack.CLI unpack -i <path_to_pac> -f <game_file> [-o output_directory]`
* Listing files: `FF16Pack.CLI list-files -i <path_to_pac>`

## Building

Requires **.NET 8.0** (VS2022), Windows, DirectStorage support.

## Format Documentation

* [010 Editor Templates](https://github.com/Nenkai/010GameTemplates/tree/main/Square%20Enix/Final%20Fantasy%2016)
* [C# Classes](https://github.com/Nenkai/FF16Pack/tree/master/FF16PackLib)

## Roadmap

* Re-packing support (in progress)

## Acknowledgements

[Vortice.Windows](https://github.com/amerkoleci/Vortice.Windows) for having a usable DirectStorage wrapper/API.
* [GisDeflate](https://github.com/sk-zk/GisDeflate) would break on a certain file (also doesn't have span interfaces),
* [Silk.NET](https://github.com/dotnet/Silk.NET) has 0 documentation, and would return obtuse windows errors such as `0x8004001` (not implemented). Very useful!

---

[yretenai](https://github.com/yretenai) - Information on the file structure

## License

MIT License. Make sure to credit if you reuse this!
