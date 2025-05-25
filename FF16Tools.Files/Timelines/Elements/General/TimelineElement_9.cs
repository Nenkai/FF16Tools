using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.General;

public class TimelineElement_9 : TimelineElementBase, ITimelineTriggerElement
{
    public TimelineElement_9()
    {
        UnionType = TimelineElementType.kTimelineElem_9;
    }

    public int AttackParamId { get; set; }
    public int Field_0x04 { get; set; }
    public int Field_0x08 { get; set; }
    public int Field_0x0C { get; set; }
    public int Field_0x10 { get; set; }
    public int Field_0x14 { get; set; }
    public int Field_0x18 { get; set; }
    public int Field_0x1C { get; set; }
    public int Field_0x20 { get; set; }
    public int Field_0x24 { get; set; }
    public int Field_0x28 { get; set; }
    public int Field_0x2C { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        AttackParamId = bs.ReadInt32();
        Field_0x04 = bs.ReadInt32();
        Field_0x08 = bs.ReadInt32();
        Field_0x0C = bs.ReadInt32();
        Field_0x10 = bs.ReadInt32();
        Field_0x14 = bs.ReadInt32();
        Field_0x18 = bs.ReadInt32();
        Field_0x1C = bs.ReadInt32();
        Field_0x20 = bs.ReadInt32();
        Field_0x24 = bs.ReadInt32();
        Field_0x28 = bs.ReadInt32();
        Field_0x2C = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteInt32(AttackParamId);
        bs.WriteInt32(Field_0x04);
        bs.WriteInt32(Field_0x08);
        bs.WriteInt32(Field_0x0C);
        bs.WriteInt32(Field_0x10);
        bs.WriteInt32(Field_0x14);
        bs.WriteInt32(Field_0x18);
        bs.WriteInt32(Field_0x1C);
        bs.WriteInt32(Field_0x20);
        bs.WriteInt32(Field_0x24);
        bs.WriteInt32(Field_0x28);
        bs.WriteInt32(Field_0x2C);
    }

    public override uint GetSize() => GetMetaSize() + 0x30;
}

