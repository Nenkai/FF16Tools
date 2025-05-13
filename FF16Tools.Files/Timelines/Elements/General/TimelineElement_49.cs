using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.General;

public class TimelineElement_49 : TimelineElementBase, ISerializableStruct
{
    public TimelineElement_49()
    {
        UnionType = TimelineElementType.kTimelineElem_49;
    }

    public int MSeqInputId { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        MSeqInputId = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteInt32(MSeqInputId);
    }

    public uint GetSize() => GetMetaSize() + 0x04;
}

