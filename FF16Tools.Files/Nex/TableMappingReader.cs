using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using FF16Tools.Files.Nex.Entities;

using Vortice.Win32;

namespace FF16Tools.Files.Nex;

/// <summary>
/// Utility class for column definition layout (.layout) files.
/// </summary>
public class TableMappingReader
{
    public static NexTableColumnLayout ReadColumnMappings(string tableName, Version version)
    {
        var columnLayout = new NexTableColumnLayout();
        int offset = 0;
        IterativeLayoutReader(columnLayout, tableName, ref offset, version);

        columnLayout.TotalInlineSize = offset;
        return columnLayout;
    }

    public static bool LayoutExists(string tableName)
    {
        string path = GetHeadersFilePath(tableName);
        return !string.IsNullOrEmpty(path);
    }

    public static string? GetHeadersFilePath(string tableName, bool checkSize = false)
    {
        string exePath = NexUtils.GetCurrentExecutingPath();
        string currentDir = Path.GetDirectoryName(exePath)!;

        string headersFilePath = Path.Combine(currentDir, "Nex", "Layouts", Path.ChangeExtension(tableName, ".layout"));
        if (File.Exists(headersFilePath))
        {
            if (checkSize)
            {
                using var fs = new FileStream(headersFilePath, FileMode.Open);
                if (fs.Length > 0)
                {
                    return headersFilePath;
                }
            }
            else
            {
                return headersFilePath;
            }
        }
        return null;
    }

    private static void IterativeLayoutReader(NexTableColumnLayout tableColumnLayout, string filename, ref int offset, Version inputVersion)
    {
        string path = GetHeadersFilePath(filename);
        using var sr = new StreamReader(path);

        var dir = Path.GetDirectoryName(filename);
        var fn = Path.GetFileNameWithoutExtension(Path.GetFileName(filename));
        int lineNumber = 0;

        Version max_version = null;
        Version min_version = new Version(1, 0, 0);

        while (!sr.EndOfStream)
        {
            lineNumber++;
            var debugln = $"{fn}:{lineNumber}";

            var line = sr.ReadLine()?.Trim();

            // support comments & skip empty lines
            if (string.IsNullOrEmpty(line) || line.StartsWith("//"))
                continue;

            var split = line.Split("|");
            var id = split[0];


            NexStructColumn column = null;
            switch (id)
            {
                case "add_column":
                    {
                        if (split.Length < 3)
                            Console.WriteLine($"Metadata error: {debugln} has malformed 'add_column' - expected 2 or 3 arguments (name, type, offset?), may break!");

                        string columnName = split[1];
                        string columnTypeStr = split[2];

                        if (columnTypeStr.EndsWith("[]") &&
                            tableColumnLayout.CustomStructDefinitions.ContainsKey(columnTypeStr.Replace("[]", "")))
                        {
                            column = new NexStructColumn()
                            {
                                Name = columnName,
                                Type = NexColumnType.CustomStructArray,
                                StructTypeName = columnTypeStr.Replace("[]", ""),
                            };
                        }
                        else
                        {
                            NexColumnType columnType = NexUtils.ColumnIdentifierToColumnType(columnTypeStr);
                            if (columnType == NexColumnType.Unknown)
                                Console.WriteLine($"Metadata error: {debugln} has malformed 'add_column' - type '{columnTypeStr}' is invalid\n" +
                                    $"Valid types: str, int8, int16, int32/int, int64, uint8, uint16, uint32/uint, uint64, float, double");

                            column = new NexStructColumn
                            {
                                Name = columnName,
                                Type = columnType
                            };
                        }


                        if (inputVersion < min_version || (max_version != null && inputVersion > max_version))
                            continue;

                        column.Offset = offset;

                        if (split.Length >= 4 && split[3] == "rel")
                        {
                            column.UsesRelativeOffset = true;
                            if (split.Length >= 5)
                                column.RelativeOffsetShift = int.Parse(split[4]);
                        }

                        offset += NexUtils.TypeToSize(column.Type);

                        tableColumnLayout.Columns.Add(column);
                        break;
                    }

                case "padding":
                    if (split.Length != 2)
                        Console.WriteLine($"Metadata error: {debugln} has malformed 'padding' - expected 1 argument (length), may break!");

                    offset += Convert.ToInt32(split[1], 16);
                    break;
                case "set_min_version":
                    {
                        if (split.Length < 2)
                            Console.WriteLine($"Metadata error: {debugln} has malformed 'set_min_version' - expected 1 arguments (version), may break!");

                        if (!Version.TryParse(split[1], out Version ver))
                            Console.WriteLine($"Metadata error: {debugln} has malformed 'set_min_version' - version is invalid - may break!");

                        min_version = ver;
                        break;
                    }

                case "set_max_version":
                    {
                        if (split.Length < 2)
                            Console.WriteLine($"Metadata error: {debugln} has malformed 'set_max_version' - expected 1 arguments (version), may break!");

                        if (!Version.TryParse(split[1], out Version ver))
                            Console.WriteLine($"Metadata error: {debugln} has malformed 'set_max_version' - version is invalid - may break!");

                        max_version = ver;
                        break;
                    }

                case "reset_min_version":
                    min_version = new Version(1, 0, 0);
                    break;
                case "reset_max_version":
                    max_version = null;
                    break;
                case "include":
                    {
                        if (split.Length != 2)
                            Console.WriteLine($"Metadata error: {debugln} has malformed 'include' - expected 1 argument (filename), may break!");

                        var headersFilename = GetHeadersFilePath($"{split[1]}.headers");
                        if (headersFilename == null)
                        {
                            Console.WriteLine($"Metadata error: unknown include file '{split[1]}.headers' - may break!");
                            continue;
                        }

                        IterativeLayoutReader(tableColumnLayout, headersFilename, ref offset, inputVersion);
                        break;
                    }

                case "define_struct":
                    string structName = split[1];

                    int fieldOffset = 0;
                    List<NexStructColumn> columns = new List<NexStructColumn>();
                    for (int i = 2; i < split.Length; i++)
                    {
                        NexColumnType type = NexUtils.ColumnIdentifierToColumnType(split[i]);
                        columns.Add(new NexStructColumn()
                        {
                            Name = split[i],
                            Type = type,
                            Offset = fieldOffset,
                        });

                        fieldOffset += NexUtils.TypeToSize(type);
                    }
                    tableColumnLayout.CustomStructDefinitions.Add(structName, columns);

                    break;
            }
        }
    }
}