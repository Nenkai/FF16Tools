using FF16Tools.Files.Nex;
using FF16Tools.Files.Nex.Entities;

using Syroot.BinaryData.Memory;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Nex;

public class NexUtils
{
    /// <summary>
    /// Reads row data using the specified column layout, and nex file buffer.
    /// </summary>
    /// <param name="tableColumnLayout"></param>
    /// <param name="nexFileBuffer"></param>
    /// <param name="rowOffset"></param>
    /// <returns></returns>
    public static List<object> ReadRow(NexTableLayout tableColumnLayout, byte[] nexFileBuffer, int rowOffset)
    {
        List<object> cells = [];

        SpanReader sr = new SpanReader(nexFileBuffer);
        sr.Position = rowOffset;

        foreach (NexStructColumn column in tableColumnLayout.Columns.Values)
        {
            int rowColumnOffset = rowOffset + (int)column.Offset;
            if (rowColumnOffset + NexUtils.TypeToSize(column.Type) > sr.Length)
                throw new Exception($"Column {column.Name} out of stream range for table. Layout outdated or file corrupted?");

            sr.Position = rowColumnOffset;

            var cell = ReadCell(ref sr, tableColumnLayout, column, rowOffset);
            cells.Add(cell);
        }

        return cells;
    }

    private static object ReadCell(ref SpanReader sr, NexTableLayout tableColumnLayout, NexStructColumn column,
        int rowOffset)
    {
        switch (column.Type)
        {
            case NexColumnType.String:
                {
                    int currentOffset = sr.Position;
                    int strOffset = sr.ReadInt32();

                    if (column.UsesRelativeOffset)
                        sr.Position = currentOffset + strOffset + column.RelativeOffsetShift;
                    else
                        sr.Position = rowOffset + strOffset;

                    string str = sr.ReadString0();
                    sr.Position = currentOffset + 4;
                    return str;
                }
            case NexColumnType.ByteArray:
            case NexColumnType.IntArray:
            case NexColumnType.UIntArray:
            case NexColumnType.FloatArray:
            case NexColumnType.StringArray:
            case NexColumnType.UnionArray:
            case NexColumnType.CustomStructArray:
                {
                    int currentOffset = sr.Position;
                    int arrayOffset = sr.ReadInt32();
                    int arrayLength = sr.ReadInt32();

                    if (column.UsesRelativeOffset)
                        sr.Position = currentOffset + arrayOffset + column.RelativeOffsetShift;
                    else
                        sr.Position = rowOffset + arrayOffset;

 
                    switch (column.Type)
                    {
                        case NexColumnType.ByteArray:
                            {
                                byte[] arr = new byte[arrayLength];
                                for (int i = 0; i < arrayLength; i++)
                                    arr[i] = sr.ReadByte();

                                sr.Position = currentOffset + 8;
                                return arr;
                            }
                        case NexColumnType.IntArray:
                            {
                                int[] arr = new int[arrayLength];
                                for (int i = 0; i < arrayLength; i++)
                                    arr[i] = sr.ReadInt32();

                                sr.Position = currentOffset + 8;
                                return arr;
                            }
                        case NexColumnType.UIntArray:
                            {
                                uint[] arr = new uint[arrayLength];
                                for (int i = 0; i < arrayLength; i++)
                                    arr[i] = sr.ReadUInt32();

                                sr.Position = currentOffset + 8;
                                return arr;
                            }
                        case NexColumnType.FloatArray:
                            {
                                float[] arr = new float[arrayLength];
                                for (int i = 0; i < arrayLength; i++)
                                    arr[i] = sr.ReadSingle();

                                sr.Position = currentOffset + 8;
                                return arr;
                            }
                        case NexColumnType.UnionArray:
                            {
                                NexUnion[] arr = new NexUnion[arrayLength];
                                for (int i = 0; i < arrayLength; i++)
                                {
                                    NexUnionType type = (NexUnionType)sr.ReadUInt16();
                                    sr.ReadInt16();
                                    int value = sr.ReadInt32();
                                    arr[i] = new NexUnion(type, value);
                                }

                                sr.Position = currentOffset + 8;
                                return arr;
                            }
                        case NexColumnType.StringArray:
                            {
                                int startOffset = sr.Position;
                                string[] arr = new string[arrayLength];
                                for (int i = 0; i < arrayLength; i++)
                                {
                                    int tempPos = sr.Position;
                                    int strOffset = sr.ReadInt32();
                                    sr.Position = startOffset + strOffset;
                                    arr[i] = sr.ReadString0();
                                    sr.Position = tempPos + 4;
                                }

                                sr.Position = currentOffset + 8;
                                return arr;
                            }
                        case NexColumnType.CustomStructArray:
                            {
                                int startOffset = sr.Position;
                                object[] arr = new object[arrayLength];

                                NexTableColumnStruct customStruct = tableColumnLayout.CustomStructDefinitions[column.StructTypeName];
                                for (int i = 0; i < arrayLength; i++)
                                {
                                    var structFields = new object[customStruct.Columns.Count];
                                    int startStructOffset = startOffset + (customStruct.TotalInlineSize * i);

                                    for (int j = 0; j < customStruct.Columns.Count; j++)
                                    {
                                        NexStructColumn field = customStruct.Columns[j];
                                        sr.Position = (int)(startStructOffset + field.Offset);
                                        var cell = ReadCell(ref sr, tableColumnLayout, field, startStructOffset);
                                        structFields[j] = cell;
                                    }

                                    arr[i] = structFields;
                                }

                                sr.Position = currentOffset + 8;
                                return arr;
                            }

                        default:
                            throw new NotImplementedException($"ReadCell: Unimplemented array type {column.Type}");
                    }
                }
            case NexColumnType.Int64:
                return sr.ReadUInt64();
            case NexColumnType.Int:
                return sr.ReadInt32();
            case NexColumnType.Short:
                return sr.ReadInt16();
            case NexColumnType.UInt:
            case NexColumnType.HexUInt:
                return sr.ReadUInt32();
            case NexColumnType.Float:
                return sr.ReadSingle();
            case NexColumnType.Double:
                return sr.ReadDouble();
            case NexColumnType.SByte:
                return sr.ReadSByte();
            case NexColumnType.Byte:
                return sr.ReadByte();
            case NexColumnType.UShort:
                return sr.ReadUInt16();
            case NexColumnType.Union:
                {
                    NexUnionType unionType = (NexUnionType)sr.ReadUInt16();
                    sr.ReadInt16();
                    int id = sr.ReadInt32();
                    return new NexUnion(unionType, id);
                }
            default:
                throw new NotImplementedException($"ReadCell: Type {column.Type} is invalid or not supported.");
        }
    }

    public static string GetCurrentExecutingPath()
    {
        string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        if (string.IsNullOrEmpty(assemblyLocation)) // This may be empty if we compiled the executable as single-file.
            assemblyLocation = Environment.GetCommandLineArgs()[0]!;

        return assemblyLocation;
    }

    /// <summary>
    /// Converts a game database value type to sqlite type identifier.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string? TypeToSQLiteTypeIdentifier(NexColumnType type)
    {
        // can't use a switch for types :(
        if (type == NexColumnType.String || type == NexColumnType.HexUInt || type == NexColumnType.Union ||
            type == NexColumnType.ByteArray || type == NexColumnType.IntArray || type == NexColumnType.UIntArray || 
            type == NexColumnType.FloatArray || type == NexColumnType.StringArray || type == NexColumnType.CustomStructArray ||
            type == NexColumnType.UnionArray)
            return "TEXT";
        else if (type == NexColumnType.Byte || type == NexColumnType.Short || type == NexColumnType.Int || type == NexColumnType.UInt || type == NexColumnType.Int64 ||
            type == NexColumnType.SByte || type == NexColumnType.UShort)
            return "INTEGER";
        else if (type == NexColumnType.Float || type == NexColumnType.Double)
            return "REAL";
        else
            return null;
    }

    /// <summary>
    /// Returns the byte size of a database value type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static int TypeToSize(NexColumnType type)
    {
        // can't use a switch for types :(
        if (type == NexColumnType.Int64 || type == NexColumnType.Double || type == NexColumnType.Union ||
            type == NexColumnType.ByteArray || type == NexColumnType.IntArray || type == NexColumnType.UIntArray || 
            type == NexColumnType.FloatArray || type == NexColumnType.StringArray || type == NexColumnType.CustomStructArray ||
            type == NexColumnType.UnionArray)
            return 8;
        else if (type == NexColumnType.Int || type == NexColumnType.UInt || type == NexColumnType.HexUInt || type == NexColumnType.Float || type == NexColumnType.HexUInt || type == NexColumnType.String)
            return 4;
        else if (type == NexColumnType.Short || type == NexColumnType.UShort)
            return 2;
        else if (type == NexColumnType.Byte || type == NexColumnType.SByte)
            return 1;

        throw new NotSupportedException($"TypeToSize: unsupported type {type}");
    }

    /// <summary>
    /// "int" -> DBColumnType.Int, etc.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public static NexColumnType ColumnIdentifierToColumnType(string str) =>
        str switch
        {
            "str" or "string" => NexColumnType.String,
            "int8" or "sbyte" => NexColumnType.SByte,
            "int16" or "short" or "2" => NexColumnType.Short,
            "int32" or "int" or "4" => NexColumnType.Int,
            // uint64 isn't supported by sqlite, so it's fine to read as int64
            "int64" or "uint64" or "long" or "ulong" or "8" => NexColumnType.Int64,
            "uint8" or "byte" or "1" => NexColumnType.Byte,
            "uint16" or "ushort" => NexColumnType.UShort,
            "uint32" or "uint" => NexColumnType.UInt,
            "hex_uint" => NexColumnType.HexUInt,
            "float" => NexColumnType.Float,
            "double" => NexColumnType.Double,
            "union" => NexColumnType.Union,
            "byte[]" => NexColumnType.ByteArray,
            "int[]" or "int32[]" => NexColumnType.IntArray,
            "uint[]" or "uint32[]" => NexColumnType.UIntArray,
            "float[]" => NexColumnType.FloatArray,
            "string[]" => NexColumnType.StringArray,
            "union[]" => NexColumnType.UnionArray,
            _ => NexColumnType.Unknown,
        };

    /// <summary>
    /// DBColumnType.Int -> "int", etc.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public static string ColumnTypeToColumnIdentifier(NexColumnType type) =>
        type switch
        {
            NexColumnType.String => "string",
            NexColumnType.SByte => "sbyte",
            NexColumnType.Short => "short",
            NexColumnType.Int => "int",
            NexColumnType.Int64 => "long",
            NexColumnType.Byte => "byte",
            NexColumnType.UShort => "ushort",
            NexColumnType.UInt => "uint",
            NexColumnType.HexUInt => "hex_uint",
            NexColumnType.Float => "float",
            NexColumnType.Double => "double",
            NexColumnType.Union => "union",
            NexColumnType.ByteArray => "byte[]",
            NexColumnType.IntArray => "int[]",
            NexColumnType.UIntArray => "uint[]",
            NexColumnType.FloatArray => "float[]",
            NexColumnType.StringArray => "string[]",
            NexColumnType.UnionArray => "union[]",
            NexColumnType.CustomStructArray => throw new ArgumentException("CustomStructArray does not have a fixed identifier."),
            _ => throw new InvalidDataException($"Unknown type {type}"),
        };

    public static bool TryParseHexUint(string hexStr, out uint value)
    {
        value = 0;

        if (string.IsNullOrWhiteSpace(hexStr))
            return false;

        ReadOnlySpan<char> noPrefixStr = hexStr.AsSpan(hexStr.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? 2 : 0);
        if (string.IsNullOrEmpty(hexStr) || !uint.TryParse(noPrefixStr, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out value))
            return false;

        return true;        
    }

    public static uint AlignValue(uint x, uint alignment)
    {
        uint mask = ~(alignment - 1);
        return (x + (alignment - 1)) & mask;
    }
}