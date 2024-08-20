using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Unpacker;

public class FF16PackDStorageChunk
{
    public ulong DataOffset { get; set; }
    public uint CompressedChunkSize { get; set; }
    public uint DecompressedSize { get; set; }
    public ushort ChunkIndex { get; set; }
    public ushort NumFilesInChunk { get; set; }

    public byte[] CachedBuffer { get; set; }

    public void FromStream(BinaryStream bs)
    {
        DataOffset = bs.ReadUInt64();
        CompressedChunkSize = bs.ReadUInt32();
        DecompressedSize = bs.ReadUInt32();
        bs.ReadUInt32(); // Unk (0)
        ChunkIndex = bs.ReadUInt16();
        NumFilesInChunk = bs.ReadUInt16();
    }

    public static int GetSize()
    {
        return 0x18;
    }
}
