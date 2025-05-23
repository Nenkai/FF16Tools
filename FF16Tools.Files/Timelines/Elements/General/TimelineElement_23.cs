using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.General;

public class TimelineElement_23 : TimelineElementBase, ISerializableStruct
{
    public TimelineElement_23()
    {
        UnionType = TimelineElementType.kTimelineElem_23;
    }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);
        bs.Position += 0x20; // Padding
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WritePadding(0x20);
    }

    public uint GetSize() => GetMetaSize() + 0x20;
}

