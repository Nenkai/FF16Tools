using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FF16Tools.Files.Timelines.Elements.General;

public class CameraAnimationRange : TimelineElementBase, ISerializableStruct
{
    public int Field_0x00 { get; set; }
    public int Field_0x04 { get; set; }
    public byte Field_0x08 { get; set; }
    public byte Field_0x09 { get; set; }
    public byte Field_0x0A { get; set; }
    public byte Field_0x0B { get; set; }
    public Sub8Struct[] Entries { get; set; } = new Sub8Struct[8];
    public int[] Unks { get; set; } = new int[12];

    public CameraAnimationRange()
    {
        UnionType = TimelineElementType.CameraAnimationRange;
        for (int i = 0; i < 8; i++)
            Entries[i] = new Sub8Struct();
    }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        Field_0x00 = bs.ReadInt32();
        Field_0x04 = bs.ReadInt32();
        Field_0x08 = bs.Read1Byte();
        Field_0x09 = bs.Read1Byte();
        Field_0x0A = bs.Read1Byte();
        Field_0x0B = bs.Read1Byte();

        for (int i = 0; i < 8; i++)
        {
            Entries[i] = new Sub8Struct();
            Entries[i].Read(bs);
        }

        for (int i = 0; i < 12; i++)
            Unks[i] = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteInt32(Field_0x00);
        bs.WriteInt32(Field_0x04);
        bs.WriteByte(Field_0x08);
        bs.WriteByte(Field_0x09);
        bs.WriteByte(Field_0x0A);
        bs.WriteByte(Field_0x0B);

        // Skip entries for now
        long entriesOffset = bs.Position;
        bs.Position += Entries.Length * 0x10;

        for (int i = 0; i < 12; i++)
            bs.WriteInt32(Unks[i]);

        bool aligned = false;
        for (int i = 0; i < 8; i++)
        {
            if (Entries[i].Data is not null)
            {
                if (!aligned)
                {
                    bs.Align(0x10, grow: true);
                    aligned = true;
                }

                bs.AddObjectPointer(Entries[i].Data!);
                bs.Write(Entries[i].Data);
            }
        }

        long lastOffset = bs.Position;
        bs.Position = entriesOffset;

        for (int i = 0; i < 8; i++)
        {
            long baseOffset = bs.Position;

            bs.WriteObjectPointer(Entries[i].Data, relativeBaseOffset: baseOffset, writeZeroIfNullOrNotFound: true);
            bs.WriteInt32(Entries[i].Data?.Length ?? 0);
            bs.WriteSingle(Entries[i].field_0x08);
            bs.WriteSingle(Entries[i].field_0x0C);
        }

        bs.Position = lastOffset;
    }

    public uint GetSize()
    {
        uint size = GetMetaSize() + 
            0x0C + // This size
            (uint)(Entries.Length * 0x10) +
            (uint)(Unks.Length * sizeof(int));

        return size;
    }

    public class Sub8Struct : ISerializableStruct
    {
        public byte[]? Data;
        public float field_0x08;
        public float field_0x0C;

        public void Read(SmartBinaryStream bs)
        {
            long startingPos = bs.Position;

            int dataOffset = bs.ReadInt32();
            int dataSize = bs.ReadInt32();
            field_0x08 = bs.ReadSingle();
            field_0x0C = bs.ReadSingle();

            long endPos = bs.Position;

            bs.Position = startingPos + dataOffset;
            Data = bs.ReadBytes(dataSize);

            bs.Position = endPos;
        }

        public void Write(SmartBinaryStream bs)
        {
            throw new NotSupportedException("Sub8Struct must be serialized separately");
        }

        public uint GetSize() => 0x10;
    }

}

