using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace FF16Tools.Pack;

public class FF16PackDStorageChunk
{
    /// <summary>
    /// Offset of the compressed data.
    /// </summary>
    public ulong DataOffset { get; set; }

    /// <summary>
    /// Size of the compressed chunk.
    /// </summary>
    public uint CompressedChunkSize { get; set; }

    /// <summary>
    /// Decompressed size of the chunk.
    /// </summary>
    public uint DecompressedSize { get; set; }

    /// <summary>
    /// Chunk index.
    /// </summary>
    public ushort ChunkIndex { get; set; }

    /// <summary>
    /// Number of files present within the chunk.
    /// </summary>
    public ushort NumFilesInChunk { get; set; }

    public byte[]? CachedBuffer { get; set; }

    public void FromStream(BinaryStream bs)
    {
        DataOffset = bs.ReadUInt64();
        CompressedChunkSize = bs.ReadUInt32();
        DecompressedSize = bs.ReadUInt32();
        bs.ReadUInt32(); // Unk (0)
        ChunkIndex = bs.ReadUInt16();
        NumFilesInChunk = bs.ReadUInt16();
    }

    public void Write(BinaryStream bs)
    {
        bs.WriteUInt64(DataOffset);
        bs.WriteUInt32(CompressedChunkSize);
        bs.WriteUInt32(DecompressedSize);
        bs.WriteUInt32(0);
        bs.WriteUInt16(ChunkIndex);
        bs.WriteUInt16(NumFilesInChunk);
    }

    public static uint GetSize()
    {
        return 0x18;
    }
}
