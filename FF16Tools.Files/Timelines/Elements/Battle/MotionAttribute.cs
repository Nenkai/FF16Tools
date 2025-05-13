using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.Battle;

public class MotionAttribute : TimelineElementBase, ISerializableStruct
{
    public MotionAttribute()
    {
        UnionType = TimelineElementType.MotionAttribute;
    }

    public int field_0x00;

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        field_0x00 = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteInt32(field_0x00);
    }

    public uint GetSize() => GetMetaSize() + 0x04;
}

