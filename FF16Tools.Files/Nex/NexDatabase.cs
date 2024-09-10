using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Nex;

public class NexDatabase
{
    /// <summary>
    /// Tables/files linked to this database.
    /// </summary>
    public Dictionary<string, NexDataFile> Tables { get; set; } = [];

    public static NexDatabase Open(string directory, ILoggerFactory logger = null)
    {
        var database = new NexDatabase();
        foreach (var path in Directory.GetFiles(directory, "*.nxd", SearchOption.TopDirectoryOnly))
        {
            try
            {
                NexDataFile nxdFile = NexDataFile.FromFile(path);
                database.Tables.Add(Path.GetFileNameWithoutExtension(path), nxdFile);
            }
            catch (Exception ex)
            {
                continue;
            }
        }

        return database;
    }

    private static NexDatabase OpenWithRoot(string directory)
    {
        string root = Path.Combine(directory, "root.nxl");
        if (!File.Exists(root))
            throw new FileNotFoundException("Root file (root.nxl) is missing in directory.");

        var database = new NexDatabase();

        string[] lines = File.ReadAllLines(root);
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] spl = lines[i].Split(',');

            if (i == 0 && spl[0] != "#NXLT")
                throw new InvalidDataException("nxl file is missing #NXLT marker.");

            string tableName = spl[0];

            if (!int.TryParse(spl[1], out int tableId))
                throw new InvalidDataException($"Invalid table id '{spl[1]}' (line {i + 1}).");

            if (!int.TryParse(spl[2], out int rowCount))
                throw new InvalidDataException($"Invalid row count '{spl[2]}' (line {i + 1}).");

            if (!int.TryParse(spl[3], out int type))
                throw new InvalidDataException($"Invalid table type '{spl[3]}' (line {i + 1}).");

            string path = Path.Combine(directory, $"{tableName}.nxd");
            NexDataFile nxdFile = NexDataFile.FromFile(path);
            database.Tables.Add(Path.GetFileNameWithoutExtension(path), nxdFile);
        }

        return database;
    }
}
