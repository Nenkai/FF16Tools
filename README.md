# FF16Pack

Unpacker for Final Fantasy XIV / 16 .pac files.

This has been tested on Final Fantasy 16 Demo (Steam/PC) files.

## Usage

* Unpacking all files: `FF16Pack.CLI unpack-all -i <path_to_pac> [-o output_directory]`
* Unpacking a specific file: `FF16Pack.CLI unpack -i <path_to_pac> -f <game_file> [-o output_directory]`
* Listing files: `FF16Pack.CLI list-files -i <path_to_pac>`

## Building

Requires **.NET 8.0** (VS2022), Windows, DirectStorage support.

## Acknowledgements

[Vortice.Windows](https://github.com/amerkoleci/Vortice.Windows) for having a usable DirectStorage wrapper/API.
* [GisDeflate](https://github.com/sk-zk/GisDeflate) would break on a certain file (also doesn't have span interfaces),
* [GDeflateNet](https://github.com/yretenai/GDeflateNet) was missing dll files that were needed by the native dll,
* [Silk.NET](https://github.com/dotnet/Silk.NET) has 0 documentation, and would return obtuse windows errors such as `0x8004001` (not implemented). Very useful!
