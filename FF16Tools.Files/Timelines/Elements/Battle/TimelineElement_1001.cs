using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.Battle;

public class TimelineElement_1001 : TimelineElementBase, ISerializableStruct
{
    public TimelineElement_1001()
    {
        UnionType = TimelineElementType.kTimelineElem_1001;
    }

    public AssetReference AssetRef { get; set; } = new();
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

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        AssetRef.Read(bs);
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
        bs.ReadCheckPadding(0x18);
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        AssetRef.Write(bs);
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
        bs.WritePadding(0x18);
    }

    public uint GetSize()
    {
        return GetMetaSize() + AssetRef.GetSize() + 0x24;
    }
}

