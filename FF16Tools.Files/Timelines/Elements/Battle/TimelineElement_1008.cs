using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.Battle;

public class TimelineElement_1008 : TimelineElementBase, ISerializableStruct
{
    public TimelineElement_1008()
    {
        UnionType = TimelineElementType.kTimelineElem_1008;
    }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);
    }

    public uint GetSize() => GetMetaSize();
}

