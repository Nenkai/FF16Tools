using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.Battle;

public class TimelineElement_1088 : TimelineElementBase, ITimelineTriggerElement
{
    public TimelineElement_1088()
    {
        UnionType = TimelineElementType.kTimelineElem_1088;
    }

    public int Field_0x00 { get; set; }
    public float Field_0x04 { get; set; }
    public float Field_0x08 { get; set; }
    public int Field_0x0C { get; set; }
    public int Field_0x10 { get; set; }
    public float Field_0x14 { get; set; }
    public float Field_0x18 { get; set; }
    public byte Field_0x1C { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        Field_0x00 = bs.ReadInt32();
        Field_0x04 = bs.ReadSingle();
        Field_0x08 = bs.ReadSingle();
        Field_0x0C = bs.ReadInt32();
        Field_0x10 = bs.ReadInt32();
        Field_0x14 = bs.ReadSingle();
        Field_0x18 = bs.ReadSingle();
        Field_0x1C = bs.Read1Byte();
        bs.ReadCheckPadding(0x03);
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteInt32(Field_0x00);
        bs.WriteSingle(Field_0x04);
        bs.WriteSingle(Field_0x08);
        bs.WriteInt32(Field_0x0C);
        bs.WriteInt32(Field_0x10);
        bs.WriteSingle(Field_0x14);
        bs.WriteSingle(Field_0x18);
        bs.WriteByte(Field_0x1C);
        bs.WritePadding(0x03);
    }

    public override uint GetSize() => GetMetaSize() + 0x20;
}

