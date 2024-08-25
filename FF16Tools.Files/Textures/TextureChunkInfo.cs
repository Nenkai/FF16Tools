﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace FF16Tools.Files.Textures;

/// <summary>
/// Represents a streamed (and potentially compressed) chunk for a texture file.
/// </summary>
public class TextureChunkInfo
{
    public uint CompressedDataOffset { get; set; }
    public uint TypeFlags { get; set; }
    public uint CompressedChunkSize { get; set; }
    public uint DecompressedChunkSize { get; set; }
    public byte UnkIndex_0x0C { get; set; }
    public bool UnkBool_0x0C { get; set; }
    public byte Unk_0x0D { get; set; }

    public void FromStream(BinaryStream bs)
    {
        CompressedDataOffset = bs.ReadUInt32();

        uint bits = bs.ReadUInt32();
        TypeFlags = bits & 0b11; // 2 bits
        CompressedChunkSize = bits >> 2; // 30 bits
        DecompressedChunkSize = bs.ReadUInt32();

        bits = bs.Read1Byte();
        UnkIndex_0x0C = (byte)(bits & 0x111_1111); // 7 bits
        UnkBool_0x0C = bits >> 7 != 0;
        Unk_0x0D = bs.Read1Byte();
        bs.ReadInt16();
    }

    public static int GetSize()
    {
        return 0x10;
    }
}