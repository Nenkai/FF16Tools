using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

using Syroot.BinaryData;

namespace FF16Tools.Files.Map;

/// <summary>
/// Gid files (unfinished)
/// </summary>
// There may be more info here.
// http://www.jp.square-enix.com/tech/library/pdf/2023_FFXVIShadowTechPaper.pdf
public class GlobalIlluminationData
{
    public byte Version { get; set; }

    public GidToc2 Toc2 = new();
    public List<GidChunkInfo> MainChunkInfos = [];

    public void Read(Stream stream)
    {
        BinaryStream bs = new BinaryStream(stream, ByteConverter.Little);
        uint magic = bs.ReadUInt32();
        Version = bs.Read1Byte();
        byte flags = bs.Read1Byte();
        byte unk = bs.Read1Byte();
        bs.Read1Byte();
        long tocSize = bs.ReadInt64();
        long toc2Size = bs.ReadInt64();

        bs.Position += tocSize;
        Toc2.Read(bs);

        bs.Position += Toc2.Part3UnkSize; // TODO

        for (int i = 0; i < Toc2.Part4_SizeOfZlibChunkInfos / 0x20; i++)
        {
            var chunkInfo = new GidChunkInfo();
            chunkInfo.Read(bs);
            MainChunkInfos.Add(chunkInfo);
        }

        long chunksOffset = bs.Position + Toc2.Part6_UnkFixedZlibChunkGroupSize;
        for (int i = 0; i < 1; i++)
        {
            bs.Position = chunksOffset + MainChunkInfos[i].ChunkOffset;
            ProcessChunkInfo(bs, MainChunkInfos[i], i);
        }

        /*
		long chunksOffset = bs.Position;
		for (int i = 0; i < Toc2.Part6_UnkFixedZlibChunkGroupInfoCount; i++)
		{
			bs.Position = chunksOffset + Toc2.UnkFixedZlibChunkGroupInfos[i].ChunkOffset;
			ProcessChunk(bs, Toc2.UnkFixedZlibChunkGroupInfos[i], i);
		}
		*/
    }

    private unsafe void ProcessChunkInfo(BinaryStream bs, GidChunkInfo chunkInfo, int index)
    {
        byte[] decompressedBytes = new byte[0x8000];

        for (int i = 0; i < 5; i++)
        {
            Span<byte> compressedBytes = bs.ReadBytes(chunkInfo.ZlibChunkSizes[i]);

            ProcessChunk(compressedBytes, decompressedBytes);
            //File.WriteAllBytes(@$"test_linqpad_{index}_{i}.bin", decompressedBytes);

            if (Version >= 6)
                bs.Align(0x04);
        }

        decompressedBytes = new byte[0x4000];
        Span<byte> compressedBytes2 = bs.ReadBytes(chunkInfo.UnkOtherZlibChunkLength);
        ProcessChunk(compressedBytes2, decompressedBytes);
        //File.WriteAllBytes($@"test_linqpad_{index}_ex1.bin", decompressedBytes);

        if (Version >= 6)
            bs.Align(0x04);

        decompressedBytes = new byte[0x8000];
        Span<byte> compressedBytes3 = bs.ReadBytes(chunkInfo.UnkOtherZlibChunkLength2);
        ProcessChunk(compressedBytes3, decompressedBytes);
        //File.WriteAllBytes(@$"test_linqpad_{index}_ex2.bin", decompressedBytes);

        if (Version >= 6)
            bs.Align(0x04);
    }

    private unsafe void ProcessChunk(Span<byte> compressedBytes, byte[] decompressedBytes)
    {
        fixed (byte* pBuffer = &compressedBytes[0])
        {
            using var stream = new UnmanagedMemoryStream(pBuffer, compressedBytes.Length);
            using var binStream = new BinaryStream(stream);

            long pos = binStream.Position;
            uint magic = binStream.ReadUInt32();
            uint dataOffset = binStream.ReadUInt32();
            uint decompressedSize = binStream.ReadUInt32();
            uint flags = binStream.ReadUInt32();
            binStream.ReadUInt32();
            binStream.ReadUInt32();

            binStream.Position = dataOffset;
            binStream.Position += 2; // Skip zlib magic

            var deflateStream = new DeflateStream(binStream, CompressionMode.Decompress);
            deflateStream.ReadExactly(decompressedBytes);
        }
    }
}

public class GidToc2
{
    public Vector3[] BBox = new Vector3[2];
    public float Part1Unk1;
    public float Part1Unk2;

    public float Part2Field_0x00;
    public float Part2Field_0x04;
    public float Part2Field_0x08;
    public float Part2Field_0x0C;
    public uint Part2Field_0x10;
    public uint Part2Field_0x14;
    public uint Part2Field_0x18;
    public uint Part2Field_0x1C;
    public uint Part2Field_0x20;
    public uint Part2Field_0x24;
    public uint GridX;
    public uint GridY;
    public uint GridZ;
    public uint Part2Field_0x34;

    public uint Part3Field_0x00;
    public uint Part3UnkSize;

    public ulong Part4_Part5Size;
    public uint Part4_field_0x04;
    public uint Part4_SizeOfZlibChunkInfos;
    public uint Part4_field_0x0C;
    public uint Part4_field_0x10;
    public uint Part4_field_0x14;
    public uint Part4_field_0x18;
    public uint Part4_field_0x1C;

    public int Part6_Part8Count;
    public int Part6_Part9Count;
    public int Part6_UnkFixedZlibChunkGroupSize;
    public int Part6_field_0x0C;
    public int Part6_Part7Count;
    public int Part6_UnkFixedZlibChunkGroupInfoCount;
    public byte Part6_field_0x18;
    public byte Part6_field_0x19;
    public byte Part6_field_0x1A;
    public byte Part6_field_0x1B;
    public int Part6_field_0x1C;
    public int Part6_field_0x20;
    public int Part6_field_0x24;
    public int Part6_field_0x28;
    public int Part6_field_0x2C;

    public List<GidToc2_Part5> Part5 = [];
    public List<GidToc2_Part7> Part7 = [];
    public List<GidToc2_Part8> Part8 = [];
    public List<GidToc2_Part9> Part9 = [];
    public List<GidChunkInfo> UnkFixedZlibChunkGroupInfos = [];

    public void Read(BinaryStream bs)
    {
        // Part 1
        BBox[0] = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        BBox[1] = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        Part1Unk1 = bs.ReadSingle();
        Part1Unk2 = bs.ReadSingle();

        // Part 2
        Part2Field_0x00 = bs.ReadSingle();
        Part2Field_0x04 = bs.ReadSingle();
        Part2Field_0x08 = bs.ReadSingle();
        Part2Field_0x0C = bs.ReadSingle();
        Part2Field_0x10 = bs.ReadUInt32();
        Part2Field_0x14 = bs.ReadUInt32();
        Part2Field_0x18 = bs.ReadUInt32();
        Part2Field_0x1C = bs.ReadUInt32();
        Part2Field_0x20 = bs.ReadUInt32();
        Part2Field_0x24 = bs.ReadUInt32();
        GridX = bs.ReadUInt32();
        GridY = bs.ReadUInt32();
        GridZ = bs.ReadUInt32();
        Part2Field_0x34 = bs.ReadUInt32();

        Part3Field_0x00 = bs.ReadUInt32();
        Part3UnkSize = bs.ReadUInt32();

        Part4_Part5Size = bs.ReadUInt64();
        Part4_SizeOfZlibChunkInfos = bs.ReadUInt32();
        Part4_field_0x0C = bs.ReadUInt32();
        Part4_field_0x10 = bs.ReadUInt32();
        Part4_field_0x14 = bs.ReadUInt32();
        Part4_field_0x18 = bs.ReadUInt32();
        Part4_field_0x1C = bs.ReadUInt32();

        for (int i = 0; i < GridX * GridY * GridZ; i++)
        {
            var ent = new GidToc2_Part5();
            ent.Read(bs);
            Part5.Add(ent);
        }


        Part6_Part8Count = bs.ReadInt32();
        Part6_Part9Count = bs.ReadInt32();
        Part6_UnkFixedZlibChunkGroupSize = bs.ReadInt32();
        Part6_field_0x0C = bs.ReadInt32();
        Part6_Part7Count = bs.ReadInt32();
        Part6_UnkFixedZlibChunkGroupInfoCount = bs.ReadInt32();
        Part6_field_0x18 = bs.Read1Byte();
        Part6_field_0x19 = bs.Read1Byte();
        Part6_field_0x1A = bs.Read1Byte();
        Part6_field_0x1B = bs.Read1Byte();
        Part6_field_0x1C = bs.ReadInt32();
        Part6_field_0x20 = bs.ReadInt32();
        Part6_field_0x24 = bs.ReadInt32();
        Part6_field_0x28 = bs.ReadInt32();
        Part6_field_0x2C = bs.ReadInt32();

        for (int i = 0; i < Part6_Part7Count; i++)
        {
            var ent = new GidToc2_Part7();
            ent.Read(bs);
            Part7.Add(ent);
        }

        for (int i = 0; i < Part6_Part8Count; i++)
        {
            var ent = new GidToc2_Part8();
            ent.Read(bs);
            Part8.Add(ent);
        }

        for (int i = 0; i < Part6_Part9Count; i++)
        {
            var ent = new GidToc2_Part9();
            ent.Read(bs);
            Part9.Add(ent);
        }

        for (int i = 0; i < Part6_UnkFixedZlibChunkGroupInfoCount; i++)
        {
            var chunkInfo = new GidChunkInfo();
            chunkInfo.Read(bs);
            UnkFixedZlibChunkGroupInfos.Add(chunkInfo);
        }
    }
}

public class GidToc2_Part5
{
    public int Field_0x00 { get; set; }
    public int Field_0x04 { get; set; }
    public int Field_0x08 { get; set; }
    public int Field_0x0C { get; set; }

    public void Read(BinaryStream bs)
    {
        Field_0x00 = bs.ReadInt32();
        Field_0x04 = bs.ReadInt32();
        Field_0x08 = bs.ReadInt32();
        Field_0x0C = bs.ReadInt32();
    }
}

public class GidToc2_Part7
{
    public int Field_0x00 { get; set; }
    public int Field_0x04 { get; set; }
    public int Field_0x08 { get; set; }
    public int Field_0x0C { get; set; }

    public void Read(BinaryStream bs)
    {
        Field_0x00 = bs.ReadInt32();
        Field_0x04 = bs.ReadInt32();
        Field_0x08 = bs.ReadInt32();
        Field_0x0C = bs.ReadInt32();
    }
}

public class GidToc2_Part8
{
    public void Read(BinaryStream bs)
    {
        bs.Position += 0x5C;
    }
}

public class GidToc2_Part9
{
    public int Field_0x00 { get; set; }
    public int Field_0x04 { get; set; }

    public void Read(BinaryStream bs)
    {
        Field_0x00 = bs.ReadInt32();
        Field_0x04 = bs.ReadInt32();
    }
}

public class GidChunkInfo
{
    public short UnkIndex;
    public short Unk;
    public uint TotalSizeOfAllChunks;
    public ushort[] ZlibChunkSizes = new ushort[5];
    public ushort UnkOtherZlibChunkLength;
    public ushort UnkOtherZlibChunkLength2;
    public uint ChunkOffset;

    public void Read(BinaryStream bs)
    {
        UnkIndex = bs.ReadInt16();
        Unk = bs.ReadInt16();
        TotalSizeOfAllChunks = bs.ReadUInt32();

        for (int i = 0; i < 5; i++)
            ZlibChunkSizes[i] = bs.ReadUInt16();

        UnkOtherZlibChunkLength = bs.ReadUInt16();
        UnkOtherZlibChunkLength2 = bs.ReadUInt16();
        bs.ReadInt16();
        ChunkOffset = bs.ReadUInt32();
        bs.ReadUInt32();
    }
}
