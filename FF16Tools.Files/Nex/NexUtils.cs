using FF16Tools.Files.Nex.Entities;

using Syroot.BinaryData.Memory;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files;

public class NexUtils
{
    /// <summary>
    /// Reads row data using the specified column layout, and nex file buffer.
    /// </summary>
    /// <param name="tableColumnLayout"></param>
    /// <param name="nexFileBuffer"></param>
    /// <param name="rowOffset"></param>
    /// <returns></returns>
    public static List<object> ReadRow(NexTableColumnLayout tableColumnLayout, byte[] nexFileBuffer, int rowOffset)
    {
        List<object> cells = [];

        SpanReader sr = new SpanReader(nexFileBuffer);
        sr.Position = rowOffset;

        int basePos = sr.Position;
        for (int j = 0; j < tableColumnLayout.Columns.Count; j++)
        {
            NexStructColumn column = tableColumnLayout.Columns[j];
            if (sr.Position >= sr.Length || sr.Position + NexUtils.TypeToSize(column.Type) > sr.Length)
                break;

            var cell = ReadCell(ref sr, tableColumnLayout, column, rowOffset);
            cells.Add(cell);
        }

        return cells;
    }

    private static object ReadCell(ref SpanReader sr, NexTableColumnLayout tableColumnLayout, NexStructColumn column,
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
            case NexColumnType.FloatArray:
            case NexColumnType.StringArray:
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
                        case NexColumnType.FloatArray:
                            {
                                float[] arr = new float[arrayLength];
                                for (int i = 0; i < arrayLength; i++)
                                    arr[i] = sr.ReadSingle();

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
                                object[] arr = new object[arrayLength];
                                
                                List<NexStructColumn> structFieldColumns = tableColumnLayout.CustomStructDefinitions[column.StructTypeName];
                                for (int i = 0; i < arrayLength; i++)
                                {
                                    var structFields = new object[structFieldColumns.Count];
                                    int startStructOffset = sr.Position;
                                    for (int j = 0; j < structFieldColumns.Count; j++)
                                    {
                                        var cell = ReadCell(ref sr, tableColumnLayout, structFieldColumns[j], startStructOffset);
                                        structFields[j] = cell;
                                    }

                                    arr[i] = structFields;
                                }

                                sr.Position = currentOffset + 8;
                                return arr;
                            }

                        default:
                            throw new NotImplementedException();
                    }
                }
            case NexColumnType.Int64:
                return sr.ReadUInt64();
            case NexColumnType.HexUInt:
                {
                    uint hexVal = sr.ReadUInt32();
                    return hexVal.ToString("X8");
                }
            case NexColumnType.Int:
                return sr.ReadInt32();
            case NexColumnType.Short:
                return sr.ReadInt16();
            case NexColumnType.UInt:
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

            default:
                throw new NotImplementedException($"Type {column.Type} is invalid or not supported.");
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
        if (type == NexColumnType.String || type == NexColumnType.HexUInt ||
            type == NexColumnType.ByteArray || type == NexColumnType.IntArray || type == NexColumnType.FloatArray || type == NexColumnType.StringArray || type == NexColumnType.CustomStructArray)
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
        if (type == NexColumnType.Int64 || type == NexColumnType.Double ||
            type == NexColumnType.ByteArray || type == NexColumnType.IntArray || type == NexColumnType.FloatArray || type == NexColumnType.StringArray || type == NexColumnType.CustomStructArray)
            return 8;
        else if (type == NexColumnType.Int || type == NexColumnType.UInt || type == NexColumnType.Float || type == NexColumnType.HexUInt || type == NexColumnType.String)
            return 4;
        else if (type == NexColumnType.Short || type == NexColumnType.UShort)
            return 2;
        else if (type == NexColumnType.Byte || type == NexColumnType.SByte)
            return 1;

        return -1;
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
            "byte[]" => NexColumnType.ByteArray,
            "int[]" or "int32[]" => NexColumnType.IntArray,
            "float[]" => NexColumnType.FloatArray,
            "string[]" => NexColumnType.StringArray,
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
            NexColumnType.ByteArray => "byte[]",
            NexColumnType.IntArray => "int[]",
            NexColumnType.FloatArray => "float[]",
            NexColumnType.StringArray => "string[]",
            NexColumnType.CustomStructArray => throw new ArgumentException("CustomStructArray does not have an identifier."),
            _ => throw new InvalidDataException($"Unknown type {type}"),
        };
}