using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.General;

public class ControlPermission : TimelineElementBase, ISerializableStruct
{
    public ControlPermission()
    {
        UnionType = TimelineElementType.ControlPermission;
    }

    public byte Field_0x00 { get; set; }
    public byte Field_0x01 { get; set; }
    public byte Field_0x02 { get; set; }
    public byte Field_0x03 { get; set; }
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
    public byte Field_0x10 { get; set; }
    public byte Field_0x11 { get; set; }
    public byte Field_0x12 { get; set; }
    public byte Field_0x13 { get; set; }
    public byte Field_0x14 { get; set; }
    public byte Field_0x15 { get; set; }
    public byte Field_0x16 { get; set; }
    public byte Field_0x17 { get; set; }
    public byte Field_0x18 { get; set; }
    public byte Field_0x19 { get; set; }
    public byte Field_0x1A { get; set; }
    public byte Field_0x1B { get; set; }
    public byte Field_0x1C { get; set; }
    public byte Field_0x1D { get; set; }
    public byte Field_0x1E { get; set; }
    public byte Field_0x1F { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        Field_0x00 = bs.Read1Byte();
        Field_0x01 = bs.Read1Byte();
        Field_0x02 = bs.Read1Byte();
        Field_0x03 = bs.Read1Byte();
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
        Field_0x10 = bs.Read1Byte();
        Field_0x11 = bs.Read1Byte();
        Field_0x12 = bs.Read1Byte();
        Field_0x13 = bs.Read1Byte();
        Field_0x14 = bs.Read1Byte();
        Field_0x15 = bs.Read1Byte();
        Field_0x16 = bs.Read1Byte();
        Field_0x17 = bs.Read1Byte();
        Field_0x18 = bs.Read1Byte();
        Field_0x19 = bs.Read1Byte();
        Field_0x1A = bs.Read1Byte();
        Field_0x1B = bs.Read1Byte();
        Field_0x1C = bs.Read1Byte();
        Field_0x1D = bs.Read1Byte();
        Field_0x1E = bs.Read1Byte();
        Field_0x1F = bs.Read1Byte();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteByte(Field_0x00);
        bs.WriteByte(Field_0x01);
        bs.WriteByte(Field_0x02);
        bs.WriteByte(Field_0x03);
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
        bs.WriteByte(Field_0x10);
        bs.WriteByte(Field_0x11);
        bs.WriteByte(Field_0x12);
        bs.WriteByte(Field_0x13);
        bs.WriteByte(Field_0x14);
        bs.WriteByte(Field_0x15);
        bs.WriteByte(Field_0x16);
        bs.WriteByte(Field_0x17);
        bs.WriteByte(Field_0x18);
        bs.WriteByte(Field_0x19);
        bs.WriteByte(Field_0x1A);
        bs.WriteByte(Field_0x1B);
        bs.WriteByte(Field_0x1C);
        bs.WriteByte(Field_0x1D);
        bs.WriteByte(Field_0x1E);
        bs.WriteByte(Field_0x1F);
    }

    public uint GetSize() => 0x20;
}

