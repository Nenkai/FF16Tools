using CommandLine;

using Syroot.BinaryData;

using FF16Tools.Pack;
using FF16Tools.Pack.Packing;

using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog;
using FF16Tools.Files.Textures;
using SixLabors.ImageSharp;

namespace FF16Tools.CLI;

public class Program
{
    public const string Version = "1.2.0";

    private static ILoggerFactory _loggerFactory;
    private static Microsoft.Extensions.Logging.ILogger _logger;

    static async Task Main(string[] args)
    {
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine($"- FF16Tools.CLI {Version} by Nenkai");
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine("- https://github.com/Nenkai");
        Console.WriteLine("- https://twitter.com/Nenkaai");
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine("");

        _loggerFactory = LoggerFactory.Create(builder => builder.AddNLog());
        _logger = _loggerFactory.CreateLogger<Program>();

        if (args.Length == 1)
        {
            if (Directory.Exists(args[0]))
            {
                foreach (var file in Directory.GetFiles(args[0]))
                {
                    if (!CanProcessFile(file))
                        continue;

                    try
                    {
                        ProcessTexFile(file);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Could not process file {path}", file);
                    }
                }

                return;
            }
            else if (File.Exists(args[0]))
            {
                if (CanProcessFile(args[0]))
                    ProcessTexFile(args[0]);

                return;
            }
        }

        var p = Parser.Default.ParseArguments<UnpackFileVerbs, UnpackAllVerbs, ListFilesVerbs, PackVerbs, TexConvVerbs>(args);
        await p.WithParsedAsync<UnpackFileVerbs>(UnpackFile);
        await p.WithParsedAsync<UnpackAllVerbs>(UnpackAll);
        await p.WithParsedAsync<PackVerbs>(PackFiles);
        p.WithParsed<ListFilesVerbs>(ListFiles);
        p.WithParsed<TexConvVerbs>(TexConv);
    }

    static async Task UnpackFile(UnpackFileVerbs verbs)
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
            await pack.ExtractFile(verbs.FileToUnpack, verbs.OutputPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unpack.");
        }
    }

    static async Task UnpackAll(UnpackAllVerbs verbs)
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
            await pack.ExtractAll(verbs.OutputPath);
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

    static async Task PackFiles(PackVerbs verbs)
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
        await builder.WriteToAsync(verbs.OutputFile);

        _logger.LogInformation("Done packing.");
    }

    public static void TexConv(TexConvVerbs verbs)
    {
        if (verbs.InputPaths.Count() == 1)
        {
            foreach (var file in Directory.GetFiles(verbs.InputPaths.First(), "*.tex", verbs.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                try
                {
                    ProcessTexFile(file);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Could not process texture file");
                }
            }
        }
        else
        {
            foreach (var file in verbs.InputPaths)
            {
                if (!File.Exists(file))
                {
                    _logger.LogError("File {file} does not exist", file);
                    continue;
                }

                try
                {
                    ProcessTexFile(file);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Could not process texture file");
                }
            }
        }
    }

    public static bool CanProcessFile(string file)
    {
        switch (Path.GetExtension(file))
        {
            case ".tex":
                return true;
        }

        return false;
    }

    public static void ProcessTexFile(string path)
    {
        using var fs = File.OpenRead(path);

        var textureFile = new TextureFile(_loggerFactory);
        textureFile.FromStream(fs);

        fs.Position = 0;

        _logger.LogInformation("Processing {path} ({numTextures} texture(s))", path, textureFile.Textures.Count);
        if (textureFile.Textures.Count > 1)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            string dir = Path.GetDirectoryName(Path.GetFullPath(path));
            string outputDir = Path.Combine(dir, $"{fileName}_textures");
            Directory.CreateDirectory(outputDir);

            for (int i = 0; i < textureFile.Textures.Count; i++)
            {
                try
                {
                    fs.Position = 0;
                    var data = textureFile.GetImageData(i, fs);
                    data.SaveAsPng(Path.Combine(outputDir, $"{i}.png"));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Could not process texture");
                }
            }
        }
        else
        {
            try
            {
                fs.Position = 0;
                var data = textureFile.GetImageData(0, fs);
                data.SaveAsPng(Path.ChangeExtension(path, ".png"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not process texture");
            }
        }
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

[Verb("tex-conv", HelpText = "Converts a tex file.")]
public class TexConvVerbs
{
    [Option('i', "input", Required = true, HelpText = "Input .tex file or folder")]
    public IEnumerable<string> InputPaths { get; set; }

    [Option('r', "recursive", HelpText = "If a folder is provided, whether to recursively convert.")]
    public bool Recursive { get; set; }
}
