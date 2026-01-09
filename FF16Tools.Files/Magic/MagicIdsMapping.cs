using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FF16Tools.Files.Magic.Factories;

namespace FF16Tools.Files.Magic;

public class MagicIdsMapping
{
    private static Dictionary<uint, string> _idToName = [];
    public static IReadOnlyDictionary<uint, string> IdToName
    {
        get
        {
            if (!Initialized)
                Read();

            return _idToName;
        }
    }

    const string DefaultPath = "Magic/Data/MagicIds.txt";

    static bool Initialized = false;

    /// <summary>
    /// Reads mapping from the specified path. If not provided, the default one will be used (Magic/Data/MagicIds.txt)
    /// </summary>
    /// <param name="path"></param>
    /// <exception cref="InvalidDataException"></exception>
    public static void Read(string? path = null)
    {
        _idToName.Clear();

        if (path is null)
        {
            string? dir = Path.GetDirectoryName(typeof(MagicIdsMapping).Assembly.Location);
            path = Path.Combine(dir ?? Directory.GetCurrentDirectory(), DefaultPath);
            if (!File.Exists(path))
            {
                path = DefaultPath;
                if (!File.Exists(path))
                    throw new FileNotFoundException($"{DefaultPath} could not be found.");
            }
        }
        else
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"{DefaultPath} could not be found.");
        }

        using var sr = File.OpenText(path);

        int lineNumber = 0;
        while (!sr.EndOfStream)
        {
            lineNumber++;

            var line = sr.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // support comments
            var commentIndex = line.IndexOf("//");
            if (commentIndex >= 0)
            {
                line = line.Substring(0, commentIndex);
                line = line.Trim();
            }

            string[] spl = line.Split('|');
            if (spl.Length <= 1)
                continue;

            switch (spl[0])
            {
                case "magic":
                    if (spl.Length != 3)
                        throw new InvalidDataException($"Expected magic|id|name, but got {spl.Length} arguments at line {lineNumber}");

                    if (!uint.TryParse(spl[1], out uint id))
                        throw new InvalidDataException($"Unable to parse {spl[1]} as magic id at line {lineNumber}");

                    _idToName.Add(id, spl[2]);
                    break;
            }
        }

        Initialized = true;
    }

    /// <summary>
    /// Used for mapping additional properties not supported by default.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="valueType"></param>
    public static void Add(uint magicId, string name)
    {
        if (!Initialized)
            Read();

        _idToName.Add(magicId, name);
    }
}
