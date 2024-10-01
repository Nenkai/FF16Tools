using System.Buffers.Binary;
using System.IO.Compression;

using Microsoft.Extensions.Logging;

using CommandLine;

using NLog;
using NLog.Extensions.Logging;

using SixLabors.ImageSharp;

using FF16Tools.Files;
using FF16Tools.Files.Save;
using FF16Tools.Files.Nex;
using FF16Tools.Files.Nex.Entities;
using FF16Tools.Files.Nex.Exporters;
using FF16Tools.Files.Nex.Managers;
using FF16Tools.Files.Textures;
using FF16Tools.Pack;
using FF16Tools.Pack.Packing;
using FF16Tools.Shared;

namespace FF16Tools.CLI;

public class Program
{
    public const string Version = "1.4.1";

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

        var p = Parser.Default.ParseArguments<UnpackFileVerbs, UnpackAllVerbs, ListFilesVerbs, PackVerbs, TexConvVerbs, NxdToSqliteVerbs, SqliteToNxdVerbs, UnpackSaveVerbs, PackSaveVerbs>(args);
        await p.WithParsedAsync<UnpackFileVerbs>(UnpackFile);
        await p.WithParsedAsync<UnpackAllVerbs>(UnpackAll);
        await p.WithParsedAsync<PackVerbs>(PackFiles);
        p.WithParsed<ListFilesVerbs>(ListFiles);
        p.WithParsed<TexConvVerbs>(TexConv);
        p.WithParsed<NxdToSqliteVerbs>(NxdToSqlite);
        p.WithParsed<SqliteToNxdVerbs>(SqliteToNxd);
        p.WithParsed<UnpackSaveVerbs>(UnpackSave);
        p.WithParsed<PackSaveVerbs>(PackSave);
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
            verbs.OutputPath = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(verbs.InputFile)), $"{inputFileName}.extracted");
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
            verbs.OutputPath = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(verbs.InputFile)), $"{inputFileName}.extracted");
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

            List<string> spl = fileName.Split('.').ToList();
            if (spl.Count >= 1)
            {
                spl.Insert(1, "diff");
                fileName = string.Join('.', spl);
            }
            else
                fileName += ".diff"; // should never be called

            verbs.OutputFile = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(verbs.InputFile)), $"{fileName}.pac");
        }

        builder.InitFromDirectory(verbs.InputFile);
        await builder.WriteToAsync(verbs.OutputFile);

        _logger.LogInformation("-> {output}", verbs.OutputFile);
        _logger.LogInformation("Done packing.");
    }

    public static void TexConv(TexConvVerbs verbs)
    {
        if (verbs.InputPaths.Count() == 1 && Directory.Exists(verbs.InputPaths.First()))
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

                    using var data = textureFile.GetAsDds(0, fs);
                    using (var outputStream = new FileStream(Path.Combine(outputDir, $"{i}.dds"), FileMode.Create))
                        outputStream.Write(data.Span);
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
                using var data = textureFile.GetAsDds(0, fs);
                using (var outputStream = new FileStream(Path.ChangeExtension(path, ".dds"), FileMode.Create))
                    outputStream.Write(data.Span);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not process texture");
            }
        }
    }

    public static void NxdToSqlite(NxdToSqliteVerbs verbs)
    {
        if (!Directory.Exists(verbs.InputFile))
        {
            _logger.LogError("Directory '{path}' does not exist", verbs.InputFile);
            return;
        }

        try
        {
            var db = NexDatabase.Open(verbs.InputFile, _loggerFactory);
            if (string.IsNullOrEmpty(verbs.OutputFile))
            {
                verbs.OutputFile = Path.ChangeExtension(verbs.InputFile, ".sqlite");
            }


            using var exporter = new NexToSQLiteExporter(db, _loggerFactory);
            exporter.ExportTables(verbs.OutputFile);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unable to export to SQLite");
        }
    }

    public static void SqliteToNxd(SqliteToNxdVerbs verbs)
    {
        if (!File.Exists(verbs.InputFile))
        {
            _logger.LogError("Directory '{path}' does not exist", verbs.InputFile);
            return;
        }

        if (string.IsNullOrEmpty(verbs.OutputFile))
        {
            string fileName = Path.GetFileNameWithoutExtension(verbs.InputFile);
            verbs.OutputFile = Path.Combine(Path.GetDirectoryName(verbs.InputFile), $"{fileName}_nxds");
        }

        try
        {
            using var importer = new SQLiteToNexImporter(verbs.InputFile, verbs.Tables.ToList(), _loggerFactory);
            importer.ReadSqlite();
            importer.SaveTo(verbs.OutputFile);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unable to import from SQLite");
        }
    }

    public static void UnpackSave(UnpackSaveVerbs verbs)
    {
        if (!File.Exists(verbs.InputFile))
        {
            _logger.LogError("File '{path}' does not exist", verbs.InputFile);
            return;
        }

        if (string.IsNullOrWhiteSpace(verbs.OutputDir))
        {
            string fileName = Path.GetFileNameWithoutExtension(verbs.InputFile);
            verbs.OutputDir = Path.Combine(Path.GetDirectoryName(verbs.InputFile), $"{fileName}.extracted");
        }

        try
        {
            _logger.LogInformation("Opening save file {file}..", verbs.InputFile);
            var faithSaveFile = FaithSaveGameData.Open(verbs.InputFile);

            Directory.CreateDirectory(verbs.OutputDir);

            foreach (KeyValuePair<string, byte[]> file in faithSaveFile.Files)
            {
                _logger.LogInformation("Writing {fileName}...", file.Key);
                File.WriteAllBytes(Path.Combine(verbs.OutputDir, file.Key), file.Value);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unable to read save file");
            return;
        }

        _logger.LogInformation("Done.");
    }

    public static void PackSave(PackSaveVerbs verbs)
    {
        if (!Directory.Exists(verbs.InputFile))
        {
            _logger.LogError("Directory '{path}' does not exist", verbs.InputFile);
            return;
        }

        if (string.IsNullOrWhiteSpace(verbs.OutputPath))
        {
            string fileName = Path.GetFileNameWithoutExtension(verbs.InputFile);
            verbs.OutputPath = Path.Combine(Path.GetDirectoryName(verbs.InputFile), $"{fileName}.png");
        }

        try
        {
            var save = new FaithSaveGameData();
            foreach (var file in Directory.GetFiles(verbs.InputFile))
            {
                _logger.LogInformation("Adding {file} to save..", file);
                save.AddFile(file);
            }

            byte[] serialized = save.WriteSaveFile();

            if (!verbs.SkipOverwritePrompt && File.Exists(verbs.OutputPath))
            {
                _logger.LogInformation("File already exists. Overwrite? [y/n]");
                if (Console.ReadKey().Key != ConsoleKey.Y)
                {
                    _logger.LogInformation("Aborting.");
                    return;
                }
                Console.WriteLine();
            }

            File.WriteAllBytes(verbs.OutputPath, serialized);
            _logger.LogInformation("Save writen to {path}.", verbs.OutputPath);

        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unable to read save file");
            return;
        }

        _logger.LogInformation("Done.");
    }


#if DEBUG
    // Debug utility for reading and re-serializing tables
    private static void DebugReBuildTables(string dir)
    {
        var db = NexDatabase.Open(dir);

        Directory.CreateDirectory("built");
        foreach (var table in db.Tables)
        {
            var layout = TableMappingReader.ReadTableLayout(table.Key, new System.Version(1, 0, 0));
            var builder = new NexDataFileBuilder(layout);

            List<NexRowInfo> rowInfos = table.Value.RowManager.GetAllRowInfos();
            if (table.Value.Type == NexTableType.DoubleKeyed)
            {
                NexDoubleKeyedRowTableManager rowSetManager = table.Value.RowManager as NexDoubleKeyedRowTableManager;
                foreach (var dk in rowSetManager.GetRowSets())
                {
                    builder.AddDoubleKeyedSet(dk.Key);
                    foreach (var subSet in dk.Value.SubSets)
                        builder.AddSubSet(dk.Key, subSet.Key);
                }
            }
            else if (table.Value.Type == NexTableType.RowSets)
            {
                NexRowSetTableManager rowSetManager = table.Value.RowManager as NexRowSetTableManager;
                foreach (var set in rowSetManager.GetRowSets())
                    builder.AddRowSet(set.Key);
            }

            for (int i = 0; i < rowInfos.Count; i++)
            {
                var row = rowInfos[i];
                List<object> cells = NexUtils.ReadRow(layout, table.Value.Buffer, row.RowDataOffset);
                builder.AddRow(row.Id, row.SubId, row.ArrayIndex, cells);
            }

            using var fs = new FileStream(Path.Combine("built", table.Key + ".nxd"), FileMode.Create);
            builder.Write(fs);
        }
    }
#endif

}

[Verb("unpack", HelpText = "Unpacks a .pac (FF16 Pack) file.")]
public class UnpackFileVerbs
{
    [Option('i', "input", Required = true, HelpText = "Input .pac file")]
    public string InputFile { get; set; }

    [Option('f', "file", Required = true, HelpText = "File to unpack.")]
    public string FileToUnpack { get; set; }

    [Option('o', "output", HelpText = "Optional. Output directory.")]
    public string OutputPath { get; set; }
}

[Verb("unpack-all", HelpText = "Unpacks all files from a .pac (FF16 Pack).")]
public class UnpackAllVerbs
{
    [Option('i', "input", Required = true, HelpText = "Input .pac file")]
    public string InputFile { get; set; }

    [Option('o', "output", HelpText = "Output directory. Optional, defaults to a folder named the same as the .pac file.")]
    public string OutputPath { get; set; }
}

[Verb("pack", HelpText = "Pack files from a directory.")]
public class PackVerbs
{
    [Option('i', "input", Required = true, HelpText = "Input directory containing files to pack.")]
    public string InputFile { get; set; }

    [Option('o', "output", HelpText = "Optional. Output '.pac' file. Optional, defaults to <filename>.diff.pac for modding purposes.")]
    public string OutputFile { get; set; }

    [Option('n', "name", HelpText = "Optional. This overrides the internal parent path specified by the archive (normally in the .path file).")]
    public string Name { get; set; }

    [Option('e', "encrypt", HelpText = "Optional. Whether to encrypt the header. Defaults to no.")]
    public bool Encrypt { get; set; }
}

[Verb("list-files", HelpText = "List files in a .pac file.")]
public class ListFilesVerbs
{
    [Option('i', "input", Required = true, HelpText = "Input .pac file")]
    public string InputFile { get; set; }
}

[Verb("nxd-to-sqlite", HelpText = "Converts nxd files to SQLite.")]
public class NxdToSqliteVerbs
{
    [Option('i', "input", Required = true, HelpText = "Input directory with .nxd files.")]
    public string InputFile { get; set; }

    [Option('o', "output", HelpText = "Output SQLite database file.")]
    public string OutputFile { get; set; }
}

[Verb("sqlite-to-nxd", HelpText = "Converts a SQLite database to nxd files.")]
public class SqliteToNxdVerbs
{
    [Option('i', "input", Required = true, HelpText = "Input SQLite file.")]
    public string InputFile { get; set; }

    [Option('o', "output", HelpText = "Output directory for .nxd files.")]
    public string OutputFile { get; set; }

    [Option('t', "tables", HelpText = "Table(s) to import. If not provided, all tables in the database be imported.")]
    public IEnumerable<string> Tables { get; set; } = [];
}

[Verb("tex-conv", HelpText = "Converts a tex file.")]
public class TexConvVerbs
{
    [Option('i', "input", Required = true, HelpText = "Input .tex file or folder")]
    public IEnumerable<string> InputPaths { get; set; }

    [Option('r', "recursive", HelpText = "If a folder is provided, whether to recursively convert.")]
    public bool Recursive { get; set; }
}

[Verb("unpack-save", HelpText = "Unpacks a save file (.png) into a folder.")]
public class UnpackSaveVerbs
{
    [Option('i', "input", Required = true, HelpText = "Input save (.png) file.")]
    public string InputFile { get; set; }

    [Option('o', "output", HelpText = "Output directory.")]
    public string OutputDir { get; set; }
}

[Verb("pack-save", HelpText = "Packs a save folder into a save file (.png).")]
public class PackSaveVerbs
{
    [Option('i', "input", Required = true, HelpText = "Input directory.")]
    public string InputFile { get; set; }

    [Option('o', "output", HelpText = "Output save (.png) file.")]
    public string OutputPath { get; set; }

    [Option('s', "skip", HelpText = "Skip overwrite prompt.")]
    public bool SkipOverwritePrompt { get; set; }
}