using FF16Tools.Files.CharaTimeline.Elements;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.CharaTimeline;

public class TimelineElement
{
    public static bool ThrowExceptionOnUnknownElement { get; set; } = false;

    public uint Field_0x00 { get; set; }
    public string Name { get; set; }
    public uint TimelineElemUnionTypeOrLayerId { get; set; }
    public uint FrameStart { get; set; }
    public uint NumFrames { get; set; }
    public uint Field_0x14 { get; set; }
    public byte Field_0x18 { get; set; }
    public byte Field_0x19 { get; set; }
    public byte Field_0x1A { get; set; }
    public byte Field_0x1B { get; set; }
    public TimelineElementDataBase Data { get; set; }

    public void Read(BinaryStream bs)
    {
        long thisPos = bs.Position;

        Field_0x00 = bs.ReadUInt32();
        int nameOffset = bs.ReadInt32();
        TimelineElemUnionTypeOrLayerId = bs.ReadUInt32();
        FrameStart = bs.ReadUInt32();
        NumFrames = bs.ReadUInt32();
        Field_0x14 = bs.ReadUInt32();
        Field_0x18 = bs.Read1Byte();
        Field_0x19 = bs.Read1Byte();
        Field_0x1A = bs.Read1Byte();
        Field_0x1B = bs.Read1Byte();
        int dataOffset = bs.ReadInt32();

        bs.Position = thisPos + nameOffset;
        Name = bs.ReadString(StringCoding.ZeroTerminated);

        bs.Position = thisPos + dataOffset;
        uint type = bs.ReadUInt32();
        bs.Position -= 4;

        Data = type switch
        {
            9 => new TimelineElement9(),
            1002 => new TimelineElement1002(),
            1064 => new TimelineElement1064(),
            _ => ThrowExceptionOnUnknownElement ? throw new NotSupportedException($"Timeline element {type} not yet supported") : null,
        };
        Data?.Read(bs);
    }
}
