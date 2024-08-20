using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace FF16PackLib;

public class FF16PackFile
{
    /// <summary>
    /// Size of the compressed file.
    /// </summary>
    public uint CompressedFileSize { get; set; }

    /// <summary>
    /// Whether this file is compressed. <see cref="ChunkedCompressionFlags"/> for the method.
    /// </summary>
    public bool IsCompressed { get; set; }

    /// <summary>
    /// Chunked compression method.
    /// </summary>
    public FF16PackChunkCompressionType ChunkedCompressionFlags { get; set; }

    /// <summary>
    /// Decompressed file size.
    /// </summary>
    public ulong DecompressedFileSize { get; set; }

    /// <summary>
    /// Data offset. This can be a direct offset if <see cref="IsCompressed"/> is false, 
    /// or an offset within a decompressed chunk if <see cref="ChunkedCompressionFlags"/> is <see cref="FF16PackChunkCompressionType.UseSharedChunk"/>.
    /// </summary>
    public ulong DataOffset { get; set; }

    /// <summary>
    /// Offset to the chunk definition if <see cref="ChunkedCompressionFlags"/> 
    /// is <see cref="FF16PackChunkCompressionType.UseMultipleChunks"/> or <see cref="FF16PackChunkCompressionType.UseSharedChunk"/>.
    /// </summary>
    public ulong ChunkDefOffset { get; set; }

    /// <summary>
    /// Offset to the game path (zero-terminated).
    /// </summary>
    public ulong FileNameOffset { get; set; }

    /// <summary>
    /// File name hash, FNV algorithm. (Not FNV-1A)
    /// </summary>
    public uint FileNameHash { get; set; }

    /// <summary>
    /// Hash of the data, CRC-32 algorithm.
    /// </summary>
    public uint CRC32Checksum { get; set; }

    /// <summary>
    /// Size of the chunk header (if applicable).
    /// </summary>
    public uint ChunkHeaderSize { get; set; }

    public void FromStream(BinaryStream bs)
    {
        CompressedFileSize = bs.ReadUInt32();
        IsCompressed = bs.ReadBoolean();
        ChunkedCompressionFlags = (FF16PackChunkCompressionType)bs.Read1Byte();
        bs.ReadUInt16();
        DecompressedFileSize = bs.ReadUInt64();
        DataOffset = bs.ReadUInt64();
        ChunkDefOffset = bs.ReadUInt64();
        FileNameOffset = bs.ReadUInt64();

        FileNameHash = bs.ReadUInt32();
        CRC32Checksum = bs.ReadUInt32();
        bs.ReadUInt32(); // Empty
        ChunkHeaderSize = bs.ReadUInt32();

    }
}

public enum FF16PackChunkCompressionType
{
    /// <summary>
    /// No compression.
    /// </summary>
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
    /// This file is in a chunk that contains data for multiple files. Used for small multiple files that fit into one single chunk
    /// </summary>
    UseSharedChunk = 3,
}
