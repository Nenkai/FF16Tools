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

    private delegate T ReadCellArrayItemDelegate<T>(ref SpanReader sr);

    private static object ReadCellArray<T>(ref SpanReader sr, int currentOffset, int arrayLength, ReadCellArrayItemDelegate<T> reader)
    {
        T[] arr = new T[arrayLength];
        for (int i = 0; i < arrayLength; i++)
            arr[i] = reader(ref sr);

        sr.Position = currentOffset + 8;
        return arr;
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
            case NexColumnType.ShortArray:
            case NexColumnType.IntArray:
            case NexColumnType.UIntArray:
            case NexColumnType.FloatArray:
            case NexColumnType.StringArray:
            case NexColumnType.NexUnionKey32Array:
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
                                return ReadCellArray(ref sr, currentOffset, arrayLength, (ref SpanReader sr) => sr.ReadByte());
                            }
                        case NexColumnType.ShortArray:
                            {
                                return ReadCellArray(ref sr, currentOffset, arrayLength, (ref SpanReader sr) => sr.ReadInt16());
                            }
                        case NexColumnType.IntArray:
                            {
                                return ReadCellArray(ref sr, currentOffset, arrayLength, (ref SpanReader sr) => sr.ReadInt32());
                            }
                        case NexColumnType.UIntArray:
                            {
                                return ReadCellArray(ref sr, currentOffset, arrayLength, (ref SpanReader sr) => sr.ReadUInt32());
                            }
                        case NexColumnType.FloatArray:
                            {
                                return ReadCellArray(ref sr, currentOffset, arrayLength, (ref SpanReader sr) => sr.ReadSingle());
                            }
                        case NexColumnType.NexUnionKey32Array:
                            {
                                NexUnionKey[] arr = new NexUnionKey[arrayLength];
                                for (int i = 0; i < arrayLength; i++)
                                {
                                    NexUnionType type = (NexUnionType)sr.ReadUInt16();
                                    sr.ReadInt16();
                                    int value = sr.ReadInt32();
                                    arr[i] = new NexUnionKey(type, value);
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

                                NexTableColumnStruct customStruct = tableColumnLayout.CustomStructDefinitions[column.StructTypeName!];
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
            case NexColumnType.NexUnionKey32:
                {
                    NexUnionType unionType = (NexUnionType)sr.ReadUInt16();
                    sr.ReadInt16();
                    int id = sr.ReadInt32();
                    return new NexUnionKey(unionType, id);
                }
            case NexColumnType.NexUnionKey16:
                {
                    NexUnionType unionType = (NexUnionType)sr.ReadUInt16();
                    short id = sr.ReadInt16();
                    return new NexUnionKey(unionType, id);
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
    public static string? TypeToSQLiteTypeIdentifier(NexColumnType type) =>
        type switch
        {
            NexColumnType.String or
            NexColumnType.HexUInt or
            NexColumnType.NexUnionKey32 or
            NexColumnType.NexUnionKey16 or
            NexColumnType.ByteArray or
            NexColumnType.ShortArray or
            NexColumnType.IntArray or
            NexColumnType.UIntArray or
            NexColumnType.FloatArray or
            NexColumnType.StringArray or
            NexColumnType.CustomStructArray or
            NexColumnType.NexUnionKey32Array => "TEXT",
            NexColumnType.Byte or
            NexColumnType.Short or
            NexColumnType.Int or
            NexColumnType.UInt or
            NexColumnType.Int64 or
            NexColumnType.SByte or
            NexColumnType.UShort => "INTEGER",
            NexColumnType.Float or
            NexColumnType.Double => "REAL",
            _ => null
        };

    /// <summary>
    /// Returns the byte size of a database value type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static int TypeToSize(NexColumnType type) =>
        type switch
        {
            NexColumnType.Int64 or
            NexColumnType.Double or
            NexColumnType.NexUnionKey32 or
            NexColumnType.ByteArray or
            NexColumnType.ShortArray or
            NexColumnType.IntArray or
            NexColumnType.UIntArray or
            NexColumnType.FloatArray or
            NexColumnType.StringArray or
            NexColumnType.CustomStructArray or
            NexColumnType.NexUnionKey32Array => 8,
            NexColumnType.Int or
            NexColumnType.UInt or
            NexColumnType.HexUInt or
            NexColumnType.Float or
            NexColumnType.NexUnionKey16 or
            NexColumnType.String => 4,
            NexColumnType.Short or
            NexColumnType.UShort => 2,
            NexColumnType.Byte or
            NexColumnType.SByte => 1,
            _ => throw new NotSupportedException($"TypeToSize: unsupported type {type}"),
        };

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
            "union" => NexColumnType.NexUnionKey32,
            "union16" => NexColumnType.NexUnionKey16,
            "byte[]" => NexColumnType.ByteArray,
            "short[]" or "int16[]" => NexColumnType.ShortArray,
            "int[]" or "int32[]" => NexColumnType.IntArray,
            "uint[]" or "uint32[]" => NexColumnType.UIntArray,
            "float[]" => NexColumnType.FloatArray,
            "string[]" => NexColumnType.StringArray,
            "union[]" => NexColumnType.NexUnionKey32Array,
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
            NexColumnType.NexUnionKey32 => "union",
            NexColumnType.NexUnionKey16 => "union16",
            NexColumnType.ByteArray => "byte[]",
            NexColumnType.ShortArray => "short[]",
            NexColumnType.IntArray => "int[]",
            NexColumnType.UIntArray => "uint[]",
            NexColumnType.FloatArray => "float[]",
            NexColumnType.StringArray => "string[]",
            NexColumnType.NexUnionKey32Array => "union[]",
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