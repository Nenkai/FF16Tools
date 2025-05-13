using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.General;

public class ControlRejectionRange : TimelineElementBase, ISerializableStruct
{
    public ControlRejectionRange()
    {
        UnionType = TimelineElementType.ControlRejectionRange;
    }

    public int Field_0x00 { get; set; }
    public byte Field_0x04 { get; set; }
    public byte Field_0x05 { get; set; }
    public byte Field_0x06 { get; set; }
    public byte Field_0x07 { get; set; }
    public byte Field_0x08 { get; set; }
    public byte Field_0x09 { get; set; }
    public byte Field_0x0A { get; set; }
    public byte Field_0x0B { get; set; }
    public byte Field_0x0C { get; set; }
    public byte Field_0x0D { get; set; }
    public byte Field_0x0E { get; set; }
    public byte Field_0x0F { get; set; }
    public int Field_0x10 { get; set; }
    public int Field_0x14 { get; set; }
    public int Field_0x18 { get; set; }

    public override void Read(SmartBinaryStream stream)
    {
        Field_0x00 = stream.ReadInt32();
        Field_0x04 = stream.Read1Byte();
        Field_0x05 = stream.Read1Byte();
        Field_0x06 = stream.Read1Byte();
        Field_0x07 = stream.Read1Byte();
        Field_0x08 = stream.Read1Byte();
        Field_0x09 = stream.Read1Byte();
        Field_0x0A = stream.Read1Byte();
        Field_0x0B = stream.Read1Byte();
        Field_0x0C = stream.Read1Byte();
        Field_0x0D = stream.Read1Byte();
        Field_0x0E = stream.Read1Byte();
        Field_0x0F = stream.Read1Byte();
        Field_0x10 = stream.ReadInt32();
        Field_0x14 = stream.ReadInt32();
        Field_0x18 = stream.ReadInt32();
    }

    public override void Write(SmartBinaryStream stream)
    {
        stream.WriteInt32(Field_0x00);
        stream.WriteByte(Field_0x04);
        stream.WriteByte(Field_0x05);
        stream.WriteByte(Field_0x06);
        stream.WriteByte(Field_0x07);
        stream.WriteByte(Field_0x08);
        stream.WriteByte(Field_0x09);
        stream.WriteByte(Field_0x0A);
        stream.WriteByte(Field_0x0B);
        stream.WriteByte(Field_0x0C);
        stream.WriteByte(Field_0x0D);
        stream.WriteByte(Field_0x0E);
        stream.WriteByte(Field_0x0F);
        stream.WriteInt32(Field_0x10);
        stream.WriteInt32(Field_0x14);
        stream.WriteInt32(Field_0x18);
    }

    public uint GetSize() => 0x1C;
}

