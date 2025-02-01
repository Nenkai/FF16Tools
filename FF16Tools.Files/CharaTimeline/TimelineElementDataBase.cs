using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.CharaTimeline;

public abstract class TimelineElementDataBase
{
    public uint Type { get; set; }
    public uint MetaField_0x04 { get; set; }
    public uint MetaField_0x08 { get; set; }
    public uint MetaField_0x0C { get; set; }

    public void ReadMeta(BinaryStream bs)
    {
        Type = bs.ReadUInt32();
        MetaField_0x04 = bs.ReadUInt32();
        MetaField_0x08 = bs.ReadUInt32();
        MetaField_0x0C = bs.ReadUInt32();
    }

    public abstract void Read(BinaryStream bs);
}
