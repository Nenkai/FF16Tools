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

    public byte Field_0x00 { get; set; }
    public byte Field_0x01 { get; set; }
    public byte Field_0x02 { get; set; }
    public byte Field_0x03 { get; set; }
    public int Field_0x04 { get; set; }
    public int Field_0x08 { get; set; }
    public int Field_0x0C { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        Field_0x00 = bs.Read1Byte();
        Field_0x01 = bs.Read1Byte();
        Field_0x02 = bs.Read1Byte();
        Field_0x03 = bs.Read1Byte();
        Field_0x04 = bs.ReadInt32();
        Field_0x08 = bs.ReadInt32();
        Field_0x0C = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.Write(Field_0x00);
        bs.Write(Field_0x01);
        bs.Write(Field_0x02);
        bs.Write(Field_0x03);
        bs.Write(Field_0x04);
        bs.Write(Field_0x08);
        bs.Write(Field_0x0C);
    }

    public uint GetSize() => GetMetaSize() + 0x10;
}
