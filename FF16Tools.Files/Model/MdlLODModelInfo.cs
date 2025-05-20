using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Model;

public class MdlLODModelInfo : ISerializableStruct
{
    public ushort MeshIndex { get; set; }
    public ushort MeshCount { get; set; }
    public uint Unknown0 { get; set; }
    public uint Unknown1 { get; set; }
    public uint TriCount { get; set; }
    public uint vBufferOffset { get; set; }
    public uint idxBufferOffset { get; set; }
    public uint DecompVertexBuffSize { get; set; }
    public uint DecompIdxBuffSize { get; set; }
    public uint DecompIdxBuffSizeMultiplied6 { get; set; }
    public uint VertexCount { get; set; }

    public uint Unknown3 { get; set; }
    public uint Unknown4 { get; set; }
    public uint Unknown5 { get; set; }
    public uint Unknown6 { get; set; }
    public uint Unknown7 { get; set; }
    public uint Unknown8 { get; set; }

    public void Read(SmartBinaryStream bs)
    {
        MeshIndex = bs.ReadUInt16();
        MeshCount = bs.ReadUInt16();
        Unknown0 = bs.ReadUInt32();
        Unknown1 = bs.ReadUInt32();
        TriCount = bs.ReadUInt32();
        vBufferOffset = bs.ReadUInt32();
        idxBufferOffset = bs.ReadUInt32();
        DecompVertexBuffSize = bs.ReadUInt32();
        DecompIdxBuffSize = bs.ReadUInt32();
        DecompIdxBuffSizeMultiplied6 = bs.ReadUInt32();
        VertexCount = bs.ReadUInt32();

        Unknown3 = bs.ReadUInt32();
        Unknown4 = bs.ReadUInt32();
        Unknown5 = bs.ReadUInt32();
        Unknown6 = bs.ReadUInt32();
        Unknown7 = bs.ReadUInt32();
        Unknown8 = bs.ReadUInt32();
    }

    public void Write(SmartBinaryStream bs)
    {
        bs.WriteUInt16(MeshIndex);
        bs.WriteUInt16(MeshCount);
        bs.WriteUInt32(Unknown0);
        bs.WriteUInt32(Unknown1);
        bs.WriteUInt32(TriCount);
        bs.WriteUInt32(vBufferOffset);
        bs.WriteUInt32(idxBufferOffset);
        bs.WriteUInt32(DecompVertexBuffSize);
        bs.WriteUInt32(DecompIdxBuffSize);
        bs.WriteUInt32(DecompIdxBuffSizeMultiplied6);
        bs.WriteUInt32(VertexCount);

        bs.WriteUInt32(Unknown3);
        bs.WriteUInt32(Unknown4);
        bs.WriteUInt32(Unknown5);
        bs.WriteUInt32(Unknown6);
        bs.WriteUInt32(Unknown7);
        bs.WriteUInt32(Unknown8);
    }

    public uint GetSize() => 0x40;
}
