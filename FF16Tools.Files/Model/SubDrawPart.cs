using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Model;

public class SubDrawPart : ISerializableStruct
{
    public uint IndexStart { get; set; }
    public uint IndexCount { get; set; }
    public uint Unknown { get; set; } //type? 0, 1, 2
    public uint Unknown2 { get; set; } //0

    public uint Unknown3 { get; set; } //0
    public uint Unknown4 { get; set; } //0
    public uint Unknown5 { get; set; } //0
    public uint Unknown6 { get; set; } //0

    public void Read(SmartBinaryStream bs)
    {
        IndexStart = bs.ReadUInt32();
        IndexCount = bs.ReadUInt32();
        Unknown = bs.ReadUInt32();
        Unknown2 = bs.ReadUInt32();

        Unknown3 = bs.ReadUInt32();
        Unknown4 = bs.ReadUInt32();
        Unknown5 = bs.ReadUInt32();
        Unknown6 = bs.ReadUInt32();
    }

    public void Write(SmartBinaryStream bs)
    {
        bs.Write(IndexStart);
        bs.Write(IndexCount);
        bs.Write(Unknown);
        bs.Write(Unknown2);

        bs.Write(Unknown3);
        bs.Write(Unknown4);
        bs.Write(Unknown5);
        bs.Write(Unknown6);
    }

    public uint GetSize() => 0x20;
}
