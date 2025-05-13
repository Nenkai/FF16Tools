using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.Battle;

public class TimelineElement_1097 : TimelineElementBase, ISerializableStruct
{
    public TimelineElement_1097()
    {
        UnionType = TimelineElementType.DisableCharaUnk;
    }

    public string? Name;
    public int field_0x04;
    public int field_0x08;

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;
        ReadMeta(bs);

        Name = bs.ReadStringPointer(basePos);
        field_0x04 = bs.ReadInt32();
        field_0x08 = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        long baseMetaPos = bs.Position;
        WriteMeta(bs);

        bs.AddStringPointer(Name, relativeBaseOffset: baseMetaPos);
        bs.WriteInt32(field_0x04);
        bs.WriteInt32(field_0x08);
    }

    public uint GetSize() => GetMetaSize() + 0x0C;
}

