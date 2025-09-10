using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Nex.Entities;

public class NexStructColumn
{
    /// <summary>
    /// Name of the column.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Type of the column.
    /// </summary>
    public required NexColumnType Type { get; set; }

    /// <summary>
    /// Name of the struct, if the type is a struct array.
    /// </summary>
    public string? StructTypeName { get; set; }

    /// <summary>
    /// Min version which this column applies.
    /// </summary>
    public Version? MinVersion { get; set; }

    /// <summary>
    /// Max version which this column applies.
    /// </summary>
    public Version? MaxVersion { get; set; }

    public bool UsesRelativeOffset { get; set; }
    public int RelativeOffsetShift { get; set; }
    public long Offset { get; set; }

    public override string ToString()
    {
        return $"{Name} ({Type} at {Offset:X8})";
    }
}

public enum NexColumnType
{
    Unknown,
    SByte,
    Byte,
    Short,
    UShort,
    Int,
    UInt,
    HexUInt,
    Float,
    Int64,
    Double,
    String,
    ByteArray,
    IntArray,
    UIntArray,
    FloatArray,
    StringArray,
    CustomStructArray,
    NexUnionKey32,
    NexUnionKey16,
    NexUnionKey32Array,
}