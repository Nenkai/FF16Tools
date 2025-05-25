using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines;

public abstract class TimelineElementBase : ISerializableStruct
{
    public TimelineElementType UnionType { get; set; }
    public uint HeaderField_0x04 { get; set; }
    public uint HeaderField_0x08 { get; set; }
    public uint HeaderField_0x0C { get; set; }

    public void ReadMeta(SmartBinaryStream bs)
    {
        UnionType = (TimelineElementType)bs.ReadUInt32();
        HeaderField_0x04 = bs.ReadUInt32();
        HeaderField_0x08 = bs.ReadUInt32();
        HeaderField_0x0C = bs.ReadUInt32();
    }

    public void WriteMeta(SmartBinaryStream bs)
    {
        bs.WriteUInt32((uint)UnionType);
        bs.WriteUInt32(HeaderField_0x04);
        bs.WriteUInt32(HeaderField_0x08);
        bs.WriteUInt32(HeaderField_0x0C);
    }

    public abstract void Read(SmartBinaryStream bs);
    public abstract void Write(SmartBinaryStream bs);
    public abstract uint GetSize();

    public uint GetMetaSize() => 0x10;
}
