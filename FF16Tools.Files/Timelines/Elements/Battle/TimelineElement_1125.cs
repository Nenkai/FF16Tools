using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.General;

public class TimelineElement_1125 : TimelineElementBase, ITimelineTriggerElement
{
    public TimelineElement_1125()
    {
        UnionType = TimelineElementType.kTimelineElem_1125;
    }

    public byte[] Field_0x00 { get; set; } // Actual structure unknown, but its 20 bytes without any offset fields

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        Field_0x00 = bs.ReadBytes(0x14);
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteBytes(Field_0x00);
    }

    public override uint GetSize() => GetMetaSize() + 0x14;
}

