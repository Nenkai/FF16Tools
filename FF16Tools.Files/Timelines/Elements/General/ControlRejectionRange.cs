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

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        Field_0x00 = bs.ReadInt32();
        Field_0x04 = bs.Read1Byte();
        Field_0x05 = bs.Read1Byte();
        Field_0x06 = bs.Read1Byte();
        Field_0x07 = bs.Read1Byte();
        Field_0x08 = bs.Read1Byte();
        Field_0x09 = bs.Read1Byte();
        Field_0x0A = bs.Read1Byte();
        Field_0x0B = bs.Read1Byte();
        Field_0x0C = bs.Read1Byte();
        Field_0x0D = bs.Read1Byte();
        Field_0x0E = bs.Read1Byte();
        Field_0x0F = bs.Read1Byte();
        Field_0x10 = bs.ReadInt32();
        Field_0x14 = bs.ReadInt32();
        Field_0x18 = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteInt32(Field_0x00);
        bs.WriteByte(Field_0x04);
        bs.WriteByte(Field_0x05);
        bs.WriteByte(Field_0x06);
        bs.WriteByte(Field_0x07);
        bs.WriteByte(Field_0x08);
        bs.WriteByte(Field_0x09);
        bs.WriteByte(Field_0x0A);
        bs.WriteByte(Field_0x0B);
        bs.WriteByte(Field_0x0C);
        bs.WriteByte(Field_0x0D);
        bs.WriteByte(Field_0x0E);
        bs.WriteByte(Field_0x0F);
        bs.WriteInt32(Field_0x10);
        bs.WriteInt32(Field_0x14);
        bs.WriteInt32(Field_0x18);
    }

    public uint GetSize() => GetMetaSize() + 0x1C;
}

