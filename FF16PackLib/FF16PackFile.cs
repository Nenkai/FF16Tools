using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace FF16PackLib;

public class FF16PackFile
{
    public uint CompressedFileSize { get; set; }
    public bool IsCompressed { get; set; }
    public FF16PackFileFlags CompressionFlags { get; set; }
    public ulong DecompressedFileSize { get; set; }
    public ulong DataOffset { get; set; } // In chunk
    public ulong ChunkDefOffset { get; set; } // used if Flags = 2 or 3
    public ulong FileNameOffset { get; set; }
    public ulong UnkHash_0x28 { get; set; }
    public uint Unk_0x30 { get; set; }
    public uint Unk_0x34 { get; set; }

    public void FromStream(BinaryStream bs)
    {
        CompressedFileSize = bs.ReadUInt32();
        IsCompressed = bs.ReadBoolean();
        CompressionFlags = (FF16PackFileFlags)bs.Read1Byte();
        bs.ReadUInt16();
        DecompressedFileSize = bs.ReadUInt64();
        DataOffset = bs.ReadUInt64();
        ChunkDefOffset = bs.ReadUInt64();
        FileNameOffset = bs.ReadUInt64();

        // TODO: Figure these out.
        UnkHash_0x28 = bs.ReadUInt64();
        Unk_0x30 = bs.ReadUInt32();
        Unk_0x34 = bs.ReadUInt32();

    }
}

public enum FF16PackFileFlags
{
    None = 0,

    /// <summary>
    /// This file uses one chunk to store its data. Used for small-medium sizes (few mbs)
    /// </summary>
    UseSpecificChunk = 1,

    /// <summary>
    /// This file uses multiple chunks for the entirety of its data. Used for medium-high file sizes (20-30+ mbs)
    /// </summary>
    UseMultipleChunks = 2,

    /// <summary>
    /// This file contains data for multiple files. Used for small multiple files that fit into one single chunk
    /// </summary>
    UseSharedChunk = 3,
}
