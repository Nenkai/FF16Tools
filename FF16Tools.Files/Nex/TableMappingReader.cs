using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using FF16Tools.Files.Nex.Entities;

namespace FF16Tools.Files.Nex;

/// <summary>
/// Utility class for column definition layout (.layout) files.
/// </summary>
public class TableMappingReader
{
    public static NexTableLayout ReadTableLayout(string tableName, Version version)
    {
        var columnLayout = new NexTableLayout();
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

    private static void IterativeLayoutReader(NexTableLayout tableColumnLayout, string filename, ref int offset, Version inputVersion)
    {
        string path = GetHeadersFilePath(filename);
        if (string.IsNullOrEmpty(path))
            throw new FileNotFoundException($"Layout file '{filename}' not found. Does it exist in the Nex/Layouts folder?");

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
                            throw new InvalidDataException($"Metadata error: {debugln} has malformed 'add_column' - expected 2 or 3 arguments (name, type, offset?)");

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
                                throw new InvalidDataException($"Metadata error: {debugln} has malformed 'add_column' - type '{columnTypeStr}' is invalid\n");

                            column = new NexStructColumn
                            {
                                Name = columnName,
                                Type = columnType
                            };
                        }


                        if (inputVersion < min_version || (max_version != null && inputVersion > max_version))
                            continue;

                        // Implicit padding. i.e current offset = 2, next type is int. align to 4 first.
                        column.Offset = ImplicitPadType(column.Type, offset);

                        if (split.Length >= 4 && split[3] == "rel")
                        {
                            column.UsesRelativeOffset = true;
                            if (split.Length >= 5)
                                column.RelativeOffsetShift = int.Parse(split[4]);
                        }

                        offset = (int)(column.Offset + NexUtils.TypeToSize(column.Type));

                        if (!tableColumnLayout.Columns.TryAdd(column.Name, column))
                            throw new InvalidDataException($"Metadata error: {debugln} has malformed 'add_column' - duplicate {column.Name} column\n");

                        break;
                    }
                case "set_table_type":
                    {
                        if (split.Length < 2)
                            throw new InvalidDataException($"Metadata error: {debugln} has malformed 'set_table_type' - expected 1 argument (type)");

                        if (!Enum.TryParse(split[1], out NexTableType tableType))
                            throw new InvalidDataException($"Metadata error: {debugln} has malformed 'set_table_type' - invalid table type");

                        tableColumnLayout.Type = tableType;
                        break;
                    }
                case "set_table_category":
                    {
                        if (split.Length < 2)
                            throw new InvalidDataException($"Metadata error: {debugln} has malformed 'set_table_category' - expected 1 argument (category)");

                        if (!Enum.TryParse(split[1], out NexTableCategory tableCategory))
                            throw new InvalidDataException($"Metadata error: {debugln} has malformed 'set_table_category' - invalid table category");

                        tableColumnLayout.Category = tableCategory;
                        break;
                    }
                case "use_base_row_id":
                    {
                        if (split.Length < 2)
                            throw new InvalidDataException($"Metadata error: {debugln} has malformed 'use_base_row_id' - expected 1 argument (bool)");

                        if (!bool.TryParse(split[1], out bool flag))
                            throw new InvalidDataException($"Metadata error: {debugln} has malformed 'use_base_row_id' - unable to parse bool");

                        tableColumnLayout.UsesBaseRowId = flag;
                        break;
                    }
                case "set_min_version":
                    {
                        if (split.Length < 2)
                            throw new InvalidDataException($"Metadata error: {debugln} has malformed 'set_min_version' - expected 1 arguments (version)");

                        if (!Version.TryParse(split[1], out Version ver))
                            throw new InvalidDataException($"Metadata error: {debugln} has malformed 'set_min_version' - version is invalid");

                        min_version = ver;
                        break;
                    }

                case "set_max_version":
                    {
                        if (split.Length < 2)
                            throw new InvalidDataException($"Metadata error: {debugln} has malformed 'set_max_version' - expected 1 arguments (version)");

                        if (!Version.TryParse(split[1], out Version ver))
                            throw new InvalidDataException($"Metadata error: {debugln} has malformed 'set_max_version' - version is invalid");

                        max_version = ver;
                        break;
                    }

                case "reset_min_version":
                    min_version = new Version(1, 0, 0);
                    break;
                case "reset_max_version":
                    max_version = null;
                    break;

                case "define_struct":
                    string structName = split[1];

                    int fieldOffset = 0;
                    List<NexStructColumn> columns = [];
                    for (int i = 2; i < split.Length; i++)
                    {
                        NexColumnType type = NexUtils.ColumnIdentifierToColumnType(split[i]);
                        fieldOffset = ImplicitPadType(type, fieldOffset);
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

                case "set_comment":
                    {
                        if (split.Length < 5)
                            throw new InvalidDataException($"Metadata error: {debugln} has malformed 'set_comment' - expected 1 arguments (key1, key2, key3, comment)");

                        if (!tableColumnLayout.Columns.TryGetValue("Comment", out _))
                            throw new InvalidDataException($"Metadata error: {debugln} has invalid 'set_comment' - table does not have a 'Comment' column");

                        string k1 = split[1];
                        uint key1 = 0;
                        if (!string.IsNullOrEmpty(k1))
                        {
                            if (!uint.TryParse(k1, out key1))
                                throw new InvalidDataException($"Metadata error: {debugln} has malformed 'set_comment' - unable to parse key1");
                        }
                        else
                        {
                            throw new InvalidDataException($"Metadata error: {debugln} has malformed 'set_comment' - missing key1");
                        }

                        string k2 = split[2];
                        uint key2 = 0;
                        if (!string.IsNullOrEmpty(k2))
                        {
                            if (!uint.TryParse(k2, out key2))
                                throw new InvalidDataException($"Metadata error: {debugln} has malformed 'set_comment' - unable to parse key2");
                        }
                        else
                        {
                           if (tableColumnLayout.Type == NexTableType.DoubleKeyed || tableColumnLayout.Type == NexTableType.TripleKeyed)
                                throw new InvalidDataException($"Metadata error: {debugln} has malformed 'set_comment' - missing key2"); 
                        }

                        string k3 = split[3];
                        uint key3 = 0;
                        if (!string.IsNullOrEmpty(k3))
                        {
                            if (!uint.TryParse(k3, out key3))
                                throw new InvalidDataException($"Metadata error: {debugln} has malformed 'set_comment' - unable to parse key3");
                        }
                        else
                        {
                            if (tableColumnLayout.Type == NexTableType.TripleKeyed)
                                throw new InvalidDataException($"Metadata error: {debugln} has malformed 'set_comment' - missing key3");
                        }

                        string comment = split[4];
                        tableColumnLayout.RowComments.Add((key1, key2, key3), comment);
                    }
                    break;

            }
        }

        if (tableColumnLayout.Type == NexTableType.Unknown || tableColumnLayout.Category == NexTableCategory.Unknown)
            throw new InvalidDataException("Layout validation error: type or category is unknown.");

        // pad whole row
        offset = (int)NexUtils.AlignValue((uint)offset, 0x04);
    }

    private static int ImplicitPadType(NexColumnType type, int offset)
    {
        switch (type)
        {
            case NexColumnType.Int:
            case NexColumnType.UInt:
            case NexColumnType.HexUInt:
            case NexColumnType.Float:
            case NexColumnType.Int64:
            case NexColumnType.Double:
            case NexColumnType.String:
            case NexColumnType.Union:
            case NexColumnType.ByteArray:
            case NexColumnType.IntArray:
            case NexColumnType.UIntArray:
            case NexColumnType.FloatArray:
            case NexColumnType.StringArray:
            case NexColumnType.UnionArray:
            case NexColumnType.CustomStructArray:
                offset = (int)NexUtils.AlignValue((uint)offset, 0x04);
                break;
        }

        return offset;
    }
}