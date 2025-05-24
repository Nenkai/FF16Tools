using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.Battle;

public class TimelineElement_1083 : TimelineElementBase, ISerializableStruct
{
    public TimelineElement_1083()
    {
        UnionType = TimelineElementType.kTimelineElem_1083;
    }

    public int CommandId { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        CommandId = bs.ReadInt32();
        bs.Position += 0x20; // Padding
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteInt32(CommandId);
        bs.WritePadding(0x20);
    }

    public uint GetSize() => GetMetaSize() + 0x24;
}

