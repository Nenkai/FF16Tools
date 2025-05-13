using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.General;

public class PlaySoundTrigger : TimelineElementBase, ISerializableStruct
{
    public PlaySoundTrigger()
    {
        UnionType = TimelineElementType.PlaySoundTrigger;
    }

    public AssetReference SoundAsset = new();
    public int field_0x08;
    public byte field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public double field_0x18;
    public double field_0x20;
    public double field_0x28;
    public int field_0x30;
    public int field_0x34;
    public int field_0x38;
    public float field_0x3C;
    public byte field_0x40;
    public float field_0x44;
    public int field_0x48;
    public int field_0x4C;

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        SoundAsset.Read(bs);
        field_0x08 = bs.ReadInt32();
        field_0x0C = bs.Read1Byte();
        bs.Position += 3;
        field_0x10 = bs.ReadInt32();
        field_0x14 = bs.ReadInt32();
        field_0x18 = bs.ReadDouble();
        field_0x20 = bs.ReadDouble();
        field_0x28 = bs.ReadDouble();
        field_0x30 = bs.ReadInt32();
        field_0x34 = bs.ReadInt32();
        field_0x38 = bs.ReadInt32();
        field_0x3C = bs.ReadSingle();
        field_0x40 = bs.Read1Byte();
        bs.Position += 3;
        field_0x44 = bs.ReadSingle();
        field_0x48 = bs.ReadInt32();
        field_0x4C = bs.ReadInt32();
        bs.Position += 0x10;
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        SoundAsset.Write(bs);
        bs.WriteInt32(field_0x08);
        bs.WriteByte(field_0x0C);
        bs.WritePadding(0x03);
        bs.WriteInt32(field_0x10);
        bs.WriteInt32(field_0x14);
        bs.WriteDouble(field_0x18);
        bs.WriteDouble(field_0x20);
        bs.WriteDouble(field_0x28);
        bs.WriteInt32(field_0x30);
        bs.WriteInt32(field_0x34);
        bs.WriteInt32(field_0x38);
        bs.WriteSingle(field_0x3C);
        bs.WriteByte(field_0x40);
        bs.WritePadding(0x03);
        bs.WriteSingle(field_0x44);
        bs.WriteInt32(field_0x48);
        bs.WriteInt32(field_0x4C);
        bs.WritePadding(0x10);
    }

    public uint GetSize() => GetMetaSize() + 0x60;
}

