using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.General;

public class BattleMessageRange : TimelineElementBase, ISerializableStruct
{
    public BattleMessageRange()
    {
        UnionType = TimelineElementType.BattleMessageRange;
    }

    public int BattleMessageId { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        BattleMessageId = bs.ReadInt32();
        bs.Position += 0x20;
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteInt32(BattleMessageId);
        bs.WritePadding(0x20);
    }

    public uint GetSize() => GetMetaSize() + 0x24;
}

