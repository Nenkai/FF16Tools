using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.Battle;

public class TimelineElement_1023 : TimelineElementBase, ISerializableStruct
{
    public TimelineElement_1023()
    {
        UnionType = TimelineElementType.kTimelineElem_1023;
    }

    public List<Sub1023Struct> SubStructs { get; set; } = [];
    public List<Sub1023Struct2> SubStructs2 { get; set; } = [];
    public AssetReference VFXAssetRef { get; set; } = new();
    public AssetReference UnkAsset2 { get; set; } = new();
    public int Field_0x20 { get; set; }
    public int Field_0x24 { get; set; }
    public int Field_0x28 { get; set; }
    public int Field_0x2C { get; set; }
    public int Field_0x30 { get; set; }
    public int Field_0x34 { get; set; }
    public int Field_0x38 { get; set; }
    public int Field_0x3C { get; set; }
    public int Field_0x40 { get; set; }
    public int Field_0x44 { get; set; }
    public int Field_0x48 { get; set; }
    public int Field_0x4C { get; set; }
    public int Field_0x50 { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        long baseOffset = bs.Position;
        ReadMeta(bs);

        long baseDataOffset = bs.Position;

        // May be reverse offset
        int structsOffset = bs.ReadInt32();
        int structsCount = bs.ReadInt32();
        int structs2Offset = bs.ReadInt32();
        int structs2Count = bs.ReadInt32();
        VFXAssetRef.Read(bs);
        UnkAsset2.Read(bs);
        Field_0x20 = bs.ReadInt32();
        Field_0x24 = bs.ReadInt32();
        Field_0x28 = bs.ReadInt32();
        Field_0x2C = bs.ReadInt32();
        Field_0x30 = bs.ReadInt32();
        Field_0x34 = bs.ReadInt32();
        Field_0x38 = bs.ReadInt32();
        Field_0x3C = bs.ReadInt32();
        Field_0x40 = bs.ReadInt32();
        Field_0x44 = bs.ReadInt32();
        Field_0x48 = bs.ReadInt32();
        Field_0x4C = bs.ReadInt32();
        Field_0x50 = bs.ReadInt32();

        if (structsOffset != 0)
        {
            bs.Position = baseDataOffset + structsOffset;
            for (int i = 0; i < structsCount; i++)
            {
                Sub1023Struct subStruct = new Sub1023Struct();
                subStruct.Read(bs);
                SubStructs.Add(subStruct);
            }
        }

        if (structs2Offset != 0)
        {
            bs.Position = baseDataOffset + structs2Offset;
            for (int i = 0; i < structs2Count; i++)
            {
                Sub1023Struct2 subStruct2 = new Sub1023Struct2();
                subStruct2.Read(bs);
                SubStructs2.Add(subStruct2);
            }
        }

        bs.Position = baseOffset + GetSize();
    }

    public override void Write(SmartBinaryStream bs)
    {
        long baseOffset = bs.Position;
        WriteMeta(bs);

        long baseDataOffset = bs.Position;
        bs.WriteObjectPointer(SubStructs, (int)baseDataOffset); // Will be reverse
        bs.WriteInt32(SubStructs.Count);
        bs.WriteInt32(0x54);
        bs.WriteInt32(SubStructs2.Count);
        VFXAssetRef.Write(bs);
        UnkAsset2.Write(bs);
        bs.WriteInt32(Field_0x20);
        bs.WriteInt32(Field_0x24);
        bs.WriteInt32(Field_0x28);
        bs.WriteInt32(Field_0x2C);
        bs.WriteInt32(Field_0x30);
        bs.WriteInt32(Field_0x34);
        bs.WriteInt32(Field_0x38);
        bs.WriteInt32(Field_0x3C);
        bs.WriteInt32(Field_0x40);
        bs.WriteInt32(Field_0x44);
        bs.WriteInt32(Field_0x48);
        bs.WriteInt32(Field_0x4C);
        bs.WriteInt32(Field_0x50);

        for (int i = 0; i < SubStructs2.Count; i++)
        {
            SubStructs2[i].Write(bs);
        }

        bs.Position = baseOffset + GetSize();
    }

    public uint GetSize() => GetMetaSize() + 0x54 + ((uint)SubStructs2.Count * 0x1C);

    public class Sub1023Struct : ISerializableStruct
    {
        public int Active { get; set; }
        public int UnkIdSlot { get; set; }
        public int EidId1 { get; set; }
        public int EidId2 { get; set; }
        public double Field_0x10 { get; set; }
        public double Field_0x18 { get; set; }
        public double Field_0x20 { get; set; }
        public double Field_0x28 { get; set; }
        public float Field_0x30 { get; set; }
        public float Field_0x34 { get; set; }

        public void Read(SmartBinaryStream bs)
        {
            Active = bs.ReadInt32();
            UnkIdSlot = bs.ReadInt32();
            EidId1 = bs.ReadInt32();
            EidId2 = bs.ReadInt32();
            Field_0x10 = bs.ReadDouble();
            Field_0x18 = bs.ReadDouble();
            Field_0x20 = bs.ReadDouble();
            Field_0x28 = bs.ReadDouble();
            Field_0x30 = bs.ReadSingle();
            Field_0x34 = bs.ReadSingle();
            bs.ReadCheckPadding(0x20);
        }

        public void Write(SmartBinaryStream bs)
        {
            bs.WriteInt32(Active);
            bs.WriteInt32(UnkIdSlot);
            bs.WriteInt32(EidId1);
            bs.WriteInt32(EidId2);
            bs.WriteDouble(Field_0x10);
            bs.WriteDouble(Field_0x18);
            bs.WriteDouble(Field_0x20);
            bs.WriteDouble(Field_0x28);
            bs.WriteSingle(Field_0x30);
            bs.WriteSingle(Field_0x34);
            bs.WritePadding(0x20);
        }

        public uint GetSize() => 0x58;
    }

    public class Sub1023Struct2 : ISerializableStruct
    {
        public int field_0x00;
        public int field_0x04;
        public float field_0x08;
        public int field_0x0C;
        public int field_0x10;
        public int field_0x14;
        public int field_0x18;

        public void Read(SmartBinaryStream bs)
        {
            field_0x00 = bs.ReadInt32();
            field_0x04 = bs.ReadInt32();
            field_0x08 = bs.ReadSingle();
            field_0x0C = bs.ReadInt32();
            field_0x10 = bs.ReadInt32();
            field_0x14 = bs.ReadInt32();
            field_0x18 = bs.ReadInt32();
        }

        public void Write(SmartBinaryStream bs)
        {
            bs.WriteInt32(field_0x00);
            bs.WriteInt32(field_0x04);
            bs.WriteSingle(field_0x08);
            bs.WriteInt32(field_0x0C);
            bs.WriteInt32(field_0x10);
            bs.WriteInt32(field_0x14);
            bs.WriteInt32(field_0x18);
        }

        public uint GetSize() => 0x1C;
    }
}

