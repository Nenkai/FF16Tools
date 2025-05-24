using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.Battle;

public class TimelineElement_1056 : TimelineElementBase, ISerializableStruct
{
    public TimelineElement_1056()
    {
        UnionType = TimelineElementType.kTimelineElem_1056;
    }

    public int FrameCountMaybe { get; set; }
    public int FrameCountMaybe2 { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        FrameCountMaybe = bs.ReadInt32();
        FrameCountMaybe2 = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteInt32(FrameCountMaybe);
        bs.WriteInt32(FrameCountMaybe2);
    }

    public uint GetSize() => GetMetaSize() + 0x08;
}

