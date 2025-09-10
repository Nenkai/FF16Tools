using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Nex.Entities;

/// <summary>
/// DevEnv.Nex.NexSerialization.Nxd.NxdUnionKey32
/// </summary>
public struct NexUnionKey
{
    public NexUnionType Type { get; set; }
    public int Value { get; set; }

    public NexUnionKey(NexUnionType type, int id)
    {
        Type = type;
        Value = id;
    }

    public static NexUnionKey FromStream(SmartBinaryStream bs)
    {
        var union = new NexUnionKey();
        union.Type = (NexUnionType)bs.ReadInt16();
        bs.ReadCheckPadding(2);
        union.Value = bs.ReadInt32();
        return union;
    }

    public void Write32(SmartBinaryStream bs)
    {
        bs.WriteUInt16((ushort)Type);
        bs.WritePadding(2);
        bs.WriteInt32(Value);
    }

    public void Write16(SmartBinaryStream bs)
    {
        bs.WriteUInt16((ushort)Type);
        bs.WriteInt16((short)Value);
    }

    public override string ToString()
    {
        return $"{Type}: {Value}";
    }
}
