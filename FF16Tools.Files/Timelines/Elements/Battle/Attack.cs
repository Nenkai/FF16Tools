using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.Battle;

public class Attack : TimelineElementBase, ISerializableStruct
{
    public Attack()
    {
        UnionType = TimelineElementType.Attack;
    }

    public int AttackParamId;
    public string? Name;
    public int field_0x08;
    public int field_0x0C;
    public string? UnkName2;
    public int field_0x14;
    public int field_0x18;
    public int field_0x1C;
    public int field_0x20;
    public int field_0x24;

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;
        ReadMeta(bs);

        AttackParamId = bs.ReadInt32();
        Name = bs.ReadStringPointer(basePos);
        field_0x08 = bs.ReadInt32();
        field_0x0C = bs.ReadInt32();
        UnkName2 = bs.ReadStringPointer(basePos);
        field_0x14 = bs.ReadInt32();
        field_0x18 = bs.ReadInt32();
        field_0x1C = bs.ReadInt32();
        field_0x20 = bs.ReadInt32();
        field_0x24 = bs.ReadInt32();

        bs.Position = basePos + GetSize();
    }

    public override void Write(SmartBinaryStream bs)
    {
        long basePos = bs.Position;
        WriteMeta(bs);

        bs.WriteInt32(AttackParamId);
        bs.AddStringPointer(Name, relativeBaseOffset: basePos);
        bs.WriteInt32(field_0x08);
        bs.WriteInt32(field_0x0C);
        bs.AddStringPointer(UnkName2, relativeBaseOffset: basePos);
        bs.WriteInt32(field_0x14);
        bs.WriteInt32(field_0x18);
        bs.WriteInt32(field_0x1C);
        bs.WriteInt32(field_0x20);
        bs.WriteInt32(field_0x24);
    }

    public uint GetSize() => GetMetaSize() + 0x28;
}

