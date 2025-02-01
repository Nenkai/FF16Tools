using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.CharaTimeline.Elements;

public class TimelineElement1064 : TimelineElementDataBase
{
    public uint AttackParamId { get; set; }
    public string Name { get; set; }
    public int Field_0x08 { get; set; }
    public int Field_0x0C { get; set; }
    public int Field_0x10 { get; set; }
    public int Field_0x14 { get; set; }
    public int Field_0x18 { get; set; }

    public override void Read(BinaryStream bs)
    {
        long thisPos = bs.Position;
        base.ReadMeta(bs);

        AttackParamId = bs.ReadUInt32();
        int nameOffset = bs.ReadInt32();
        Field_0x08 = bs.ReadInt32();
        Field_0x0C = bs.ReadInt32();
        Field_0x0C = bs.ReadInt32();
        Field_0x14 = bs.ReadInt32();
        Field_0x18 = bs.ReadInt32();

        bs.Position = thisPos + nameOffset;
        Name = bs.ReadString(StringCoding.ZeroTerminated);
    }
}
