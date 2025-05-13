using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.Battle;

public class TimelineElement_1035 : TimelineElementBase, ISerializableStruct
{
    public TimelineElement_1035()
    {
        UnionType = TimelineElementType.kTimelineElem_1035;
    }

    public AssetReference AnimationAsset = new();
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;
    public int field_0x1C;

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        AnimationAsset.Read(bs);
        field_0x08 = bs.ReadInt32();
        field_0x0C = bs.ReadInt32();
        field_0x10 = bs.ReadInt32();
        field_0x14 = bs.ReadInt32();
        field_0x18 = bs.ReadInt32();
        field_0x1C = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        AnimationAsset.Write(bs);
        bs.WriteInt32(field_0x08);
        bs.WriteInt32(field_0x0C);
        bs.WriteInt32(field_0x10);
        bs.WriteInt32(field_0x14);
        bs.WriteInt32(field_0x18);
        bs.WriteInt32(field_0x1C);
    }

    public uint GetSize() => GetMetaSize() + 0x20;
}

