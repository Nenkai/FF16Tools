using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Chara;

public class Asset : ISerializableStruct
{
    public int field_0x00;
    public int field_0x04;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public string? FileName;
    public string? Path;

    public void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        field_0x00 = bs.ReadInt32();
        field_0x04 = bs.ReadInt32();
        field_0x08 = bs.ReadInt32();
        field_0x0C = bs.ReadInt32();
        field_0x10 = bs.ReadInt32();
        FileName = bs.ReadStringPointer(relativeBaseOffset: basePos);
        Path = bs.ReadStringPointer(relativeBaseOffset: basePos);
    }

    public void Write(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        bs.WriteInt32(field_0x00);
        bs.WriteInt32(field_0x04);
        bs.WriteInt32(field_0x08);
        bs.WriteInt32(field_0x0C);
        bs.WriteInt32(field_0x10);
        bs.AddStringPointer(FileName, relativeBaseOffset: basePos);
        bs.AddStringPointer(Path, relativeBaseOffset: basePos);
    }

    public uint GetSize() => 0x1C;
}
