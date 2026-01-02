using FF16Tools.Files.Magic;
using FF16Tools.Pack;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.CLI.Misc;

public class MagicDumper
{
    internal static void Dump(string extractedCharaDir)
    {
        foreach (var packFile in Directory.EnumerateFiles(extractedCharaDir, "*.*", SearchOption.AllDirectories))
        {
            if (packFile.EndsWith(".pac"))
            {
                var pac = FF16Pack.Open(packFile, "faith");
                foreach (var file in pac.Files)
                {
                    if (file.Key.EndsWith(".magic"))
                    {
                        byte[] data = pac.GetFileDataBytes(file.Key);

                        MagicFile magic = new MagicFile();
                        magic.Read(new MemoryStream(data));

                        DumpMagic(file.Key, magic);
                    }
                }
            }
        }

        List<string> lines = [];
        lines.Add("//----------------- Operation Info");
        foreach (var op in operationInfos)
        {
            lines.Add($"# Operation {op.Key}");
            lines.Add($"Uses properties: {string.Join(", ", op.Value.UsedProperties)}");

            lines.Add("Used by:");
            foreach (var user in op.Value.UsedBy)
            {
                lines.Add($"  - {user}");
            }
            lines.Add(string.Empty);
        }
        lines.Add(string.Empty);

        lines.Add("//----------------- Property Info");
        lines.AddRange(propertyInfos.Select(kv => $"Prop {kv.Key} used by ops: {string.Join(", ", kv.Value.UsedBy)}"));
        lines.Add(string.Empty);

        lines.Add("//----------------- Magic File Info");
        foreach (var magicFile in magicFileInfos)
        {
            lines.Add($"# {magicFile.Key}");
            lines.Add($"Uses magic ids:");
            foreach (var user in magicFile.Value)
            {
                lines.Add($"- MagicID {user}");
            }
            lines.Add(string.Empty);
        }
        File.WriteAllLines("magic_info.txt", lines);

    }

    private static SortedDictionary<string, SortedSet<int>> magicFileInfos = [];
    private static SortedDictionary<int, PropertyInfo> propertyInfos = [];
    private static SortedDictionary<int, OperationInfo> operationInfos = [];

    private static void DumpMagic(string fileName, MagicFile magic)
    {
        foreach (var entry in magic.MagicEntries.Values)
        {
            if (magicFileInfos.TryGetValue(fileName, out SortedSet<int>? magicIds))
                magicIds.Add((int)entry.Id);
            else
                magicFileInfos.Add(fileName, [(int)entry.Id]);

            foreach (var group in entry.OperationGroupList.OperationGroups)
            {
                foreach (var operation in group.OperationList.Operations)
                {
                    if (!operationInfos.TryGetValue((int)operation.Type, out OperationInfo? opInfo))
                    {
                        opInfo = new();
                        operationInfos.Add((int)operation.Type, opInfo);
                    }

                    foreach (var property in operation.Properties)
                    {
                        opInfo.UsedProperties.Add((uint)property.Type);

                        if (!propertyInfos.TryGetValue((int)property.Type, out PropertyInfo? propInfo))
                        {
                            propInfo = new();
                            propertyInfos.Add((int)property.Type, propInfo);
                        }

                        propInfo.UsedBy.Add((uint)operation.Type);
                    }

                    
                    opInfo.UsedBy.Add(fileName);
                }
            }
        }
    }
}

public record PropertyInfo
{
    public SortedSet<uint> UsedBy { get; set; } = [];
}

public record OperationInfo
{
    public SortedSet<uint> UsedProperties { get; set; } = [];
    public SortedSet<string> UsedBy { get; set; } = [];
}
