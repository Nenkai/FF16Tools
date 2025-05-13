using FF16Tools.Files.Timelines.Chara;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.General;

public class TimelineElement_5 : TimelineElementBase, ISerializableStruct
{
    public TimelineElement_5()
    {
        UnionType = TimelineElementType.kTimelineElem_5;
    }

    public byte field_0x00;
    public byte field_0x01;
    public byte field_0x02;
    public byte field_0x03;
    public int field_0x04;
    public int field_0x08;
    public int field_0x0C;

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        field_0x00 = bs.Read1Byte();
        field_0x01 = bs.Read1Byte();
        field_0x02 = bs.Read1Byte();
        field_0x03 = bs.Read1Byte();
        field_0x04 = bs.ReadInt32();
        field_0x08 = bs.ReadInt32();
        field_0x0C = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.Write(field_0x00);
        bs.Write(field_0x01);
        bs.Write(field_0x02);
        bs.Write(field_0x03);
        bs.Write(field_0x04);
        bs.Write(field_0x08);
        bs.Write(field_0x0C);
    }

    public uint GetSize() => GetMetaSize() + 0x10;
}
