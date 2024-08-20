using FF16PackLib;
using FF16PackLib.Packing;

namespace FF16PackLib.CLI;

using CommandLine;

public class Program
{
    public const string Version = "1.0.1";

    static void Main(string[] args)
    {
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine($"- FF16Pack.CLI {Version} by Nenkai");
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine("- https://github.com/Nenkai");
        Console.WriteLine("- https://twitter.com/Nenkaai");
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine("");

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
            Console.WriteLine($"ERROR: File '{verbs.InputFile}' does not exist");
            return;
        }

        if (string.IsNullOrEmpty(verbs.OutputPath))
        {
            string inputFileName = Path.GetFileNameWithoutExtension(verbs.InputFile);
            verbs.OutputPath = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(verbs.InputFile)), $"{inputFileName}_extracted");
        }

        try
        {
            using var pack = FF16Pack.Open(verbs.InputFile);
            DumpPack(pack);

            Console.WriteLine();
            Console.WriteLine("Starting unpack process.");
            pack.ExtractFile(verbs.FileToUnpack, verbs.OutputPath);
            Console.WriteLine("Done.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to extract. {ex}");
        }
    }

    static void UnpackAll(UnpackAllVerbs verbs)
    {
        if (!File.Exists(verbs.InputFile))
        {
            Console.WriteLine($"ERROR: File '{verbs.InputFile}' does not exist.");
            return;
        }

        if (string.IsNullOrEmpty(verbs.OutputPath))
        {
            string inputFileName = Path.GetFileNameWithoutExtension(verbs.InputFile);
            verbs.OutputPath = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(verbs.InputFile)), $"{inputFileName}_extracted");
        }

        try
        {
            using var pack = FF16Pack.Open(verbs.InputFile);
            DumpPack(pack);

            Console.WriteLine();
            Console.WriteLine("Starting unpack process.");
            pack.ExtractAll(verbs.OutputPath);
            Console.WriteLine("Done.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to extract. {ex}");
        }
    }

    static void ListFiles(ListFilesVerbs verbs)
    {
        if (!File.Exists(verbs.InputFile))
        {
            Console.WriteLine($"ERROR: File '{verbs.InputFile}' does not exist.");
            return;
        }

        try
        {
            using var pack = FF16Pack.Open(verbs.InputFile);
            DumpPack(pack);
            Console.WriteLine();

            string inputFileName = Path.GetFileNameWithoutExtension(verbs.InputFile);
            string outputPath = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(verbs.InputFile)), $"{inputFileName}_files.txt");
            pack.ListFiles(outputPath);
            Console.WriteLine($"Done. ({outputPath})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to read pack. {ex}");
        }
    }

    static void PackFiles(PackVerbs verbs)
    {
        if (!Directory.Exists(verbs.InputFile))
        {
            Console.WriteLine($"ERROR: Directory '{verbs.InputFile}' does not exist.");
            return;
        }

        var builder = new FF16PackBuilder(new PackBuildOptions()
        {
            Encrypt = verbs.Encrypt,
            Name = verbs.Name,
        });

        if (string.IsNullOrEmpty(verbs.OutputFile))
        {
            string fileName = Path.GetFileNameWithoutExtension(verbs.InputFile);
            verbs.OutputFile = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(verbs.InputFile)), $"{fileName}.pac");
        }

        builder.InitFromDirectory(verbs.InputFile);
        builder.WriteTo(verbs.OutputFile);

        Console.WriteLine("Done.");
    }

    public static void DumpPack(FF16Pack pack)
    {
        Console.WriteLine($"Pack Info:");
        Console.WriteLine($"- Num Files: {pack.GetNumFiles()}");
        Console.WriteLine($"- Chunks: {pack.GetNumChunks()}");
        Console.WriteLine($"- Header Encryption: {pack.HeaderEncrypted}");
        Console.WriteLine($"- Uses Chunks: {pack.UseChunks}");
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
