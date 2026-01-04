using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FF16Tools.Files.Magic.Factories;

namespace FF16Tools.Files.Magic;

public class MagicPropertyValueTypeMapping
{
    public static Dictionary<MagicPropertyType, MagicPropertyValueType> TypeToValueType { get; private set; } = [];

    const string DefaultPath = "Magic/MagicPropertyValueTypes.txt";

    static MagicPropertyValueTypeMapping()
    {
        Read();
    }

    /// <summary>
    /// Reads mapping from the specified path. If not provided, the default one will be used (Magic/MagicPropertyValueTypes.txt)
    /// </summary>
    /// <param name="path"></param>
    /// <exception cref="InvalidDataException"></exception>
    public static void Read(string? path = null)
    {
        TypeToValueType.Clear();

        if (path is null)
        {
            string? dir = Path.GetDirectoryName(typeof(MagicPropertyValueTypeMapping).Assembly.Location);
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
                case "type":
                    if (spl.Length != 3)
                        throw new InvalidDataException($"Expected type|<prop type>|<prop value type>, but got {spl.Length} arguments at line {lineNumber}");

                    if (!int.TryParse(spl[1], out int propType))
                        throw new InvalidDataException($"Unable to parse {spl[1]} as property type at line {lineNumber}");

                    if (!Enum.TryParse(spl[2], out MagicPropertyValueType valueType))
                        throw new InvalidDataException($"Unable to parse {spl[2]} as property value type {valueType} at line {lineNumber}");

                    TypeToValueType.Add((MagicPropertyType)propType, valueType);
                    break;
            }
        }
    }

    /// <summary>
    /// Used for mapping additional properties not supported by default.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="valueType"></param>
    public static void Add(MagicPropertyType type, MagicPropertyValueType valueType)
    {
        TypeToValueType.TryAdd(type, valueType);
    }
}
