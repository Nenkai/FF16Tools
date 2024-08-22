using CommandLine;

using Syroot.BinaryData;

using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using FF16PackLib;
using FF16PackLib.Packing;

using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog;

namespace FF16PackLib.CLI;

public class Program
{
    public const string Version = "1.0.1";

    private static ILoggerFactory _loggerFactory;
    private static Microsoft.Extensions.Logging.ILogger _logger;

    static void Main(string[] args)
    {
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine($"- FF16Pack.CLI {Version} by Nenkai");
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine("- https://github.com/Nenkai");
        Console.WriteLine("- https://twitter.com/Nenkaai");
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine("");

        _loggerFactory = LoggerFactory.Create(builder => builder.AddNLog());
        _logger = _loggerFactory.CreateLogger<Program>();

        var p = Parser.Default.ParseArguments<UnpackFileVerbs, UnpackAllVerbs, ListFilesVerbs, PackVerbs>(args)
            .WithParsed<UnpackFileVerbs>(UnpackFile)
            .WithParsed<UnpackAllVerbs>(UnpackAll)
            .WithParsed<ListFilesVerbs>(ListFiles)
            .WithParsed<PackVerbs>(PackFiles);
    }

    static void UnpackFile(UnpackFileVerbs verbs)
    {
        if (!File.Exists(verbs.InputFile))
        {
            _logger.LogError("File '{path}' does not exist", verbs.InputFile);
            return;
        }

        if (string.IsNullOrEmpty(verbs.OutputPath))
        {
            string inputFileName = Path.GetFileNameWithoutExtension(verbs.InputFile);
            verbs.OutputPath = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(verbs.InputFile)), $"{inputFileName}_extracted");
        }

        try
        {
            using var pack = FF16Pack.Open(verbs.InputFile, _loggerFactory);
            pack.DumpInfo();

            _logger.LogInformation("Starting unpack process.");
            pack.ExtractFile(verbs.FileToUnpack, verbs.OutputPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unpack.");
        }
    }

    static void UnpackAll(UnpackAllVerbs verbs)
    {
        if (!File.Exists(verbs.InputFile))
        {
            _logger.LogError("File '{path}' does not exist", verbs.InputFile);
            return;
        }

        if (string.IsNullOrEmpty(verbs.OutputPath))
        {
            string inputFileName = Path.GetFileNameWithoutExtension(verbs.InputFile);
            verbs.OutputPath = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(verbs.InputFile)), $"{inputFileName}_extracted");
        }

        try
        {
            using var pack = FF16Pack.Open(verbs.InputFile, _loggerFactory);
            pack.DumpInfo();

            _logger.LogInformation("Starting unpack process.");
            pack.ExtractAll(verbs.OutputPath);
            _logger.LogInformation("Done.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unpack.");
        }
    }

    static void ListFiles(ListFilesVerbs verbs)
    {
        if (!File.Exists(verbs.InputFile))
        {
            _logger.LogError("File '{path}' does not exist", verbs.InputFile);
            return;
        }

        try
        {
            using var pack = FF16Pack.Open(verbs.InputFile, _loggerFactory);
            pack.DumpInfo();

            string inputFileName = Path.GetFileNameWithoutExtension(verbs.InputFile);
            string outputPath = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(verbs.InputFile)), $"{inputFileName}_files.txt");
            pack.ListFiles(outputPath);
            _logger.LogInformation("Done. ({path})", outputPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read pack.");
        }
    }

    static void PackFiles(PackVerbs verbs)
    {
        if (!Directory.Exists(verbs.InputFile))
        {
            _logger.LogError("Directory '{path}' does not exist", verbs.InputFile);
            return;
        }

        var builder = new FF16PackBuilder(new PackBuildOptions()
        {
            Encrypt = verbs.Encrypt,
            Name = verbs.Name,
        }, _loggerFactory);

        if (string.IsNullOrEmpty(verbs.OutputFile))
        {
            string fileName = Path.GetFileNameWithoutExtension(verbs.InputFile);
            verbs.OutputFile = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(verbs.InputFile)), $"{fileName}.pac");
        }

        builder.InitFromDirectory(verbs.InputFile);
        builder.WriteTo(verbs.OutputFile);

        _logger.LogInformation("Done packing.");
    }
}

[Verb("unpack", HelpText = "Unpacks a .pac (FF16 Pack) file.")]
public class UnpackFileVerbs
{
    [Option('i', "input", Required = true, HelpText = "Input .pac file")]
    public string InputFile { get; set; }

    [Option('f', "file", Required = true, HelpText = "File to unpack.")]
    public string FileToUnpack { get; set; }

    [Option('o', "output", HelpText = "Output file. Optional, defaults to a folder named the same as the .pac file.")]
    public string OutputPath { get; set; }
}

[Verb("unpack-all", HelpText = "Unpacks all files from a .pac (FF16 Pack).")]
public class UnpackAllVerbs
{
    [Option('i', "input", Required = true, HelpText = "Input .pac file")]
    public string InputFile { get; set; }

    [Option('o', "output", HelpText = "Output file. Optional, defaults to a folder named the same as the .pac file.")]
    public string OutputPath { get; set; }
}

[Verb("pack", HelpText = "Pack files from a directory.")]
public class PackVerbs
{
    [Option('i', "input", Required = true, HelpText = "Input directory")]
    public string InputFile { get; set; }

    [Option('o', "output", HelpText = "Output '.pac' file.")]
    public string OutputFile { get; set; }

    [Option('n', "name", HelpText = "Name of the pack file.")]
    public string Name { get; set; }

    [Option('e', "encrypt", HelpText = "Whether to encrypt the header.")]
    public bool Encrypt { get; set; }
}

[Verb("list-files", HelpText = "List files in a .pac file.")]
public class ListFilesVerbs
{
    [Option('i', "input", Required = true, HelpText = "Input .pac file")]
    public string InputFile { get; set; }
}
