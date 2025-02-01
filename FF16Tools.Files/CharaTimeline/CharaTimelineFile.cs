using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace FF16Tools.Files.CharaTimeline;

public class CharaTimelineFile
{
    public uint TotalFrames { get; set; }

    public List<TimelineElement> Elements { get; set; } = [];

    public void Read(string file)
    {
        using var fs = File.OpenRead(file);
        Read(fs);
    }

    public void Read(Stream stream)
    {
        var bs = new BinaryStream(stream);
        uint magic = bs.ReadUInt32();

        if (magic != 0x4C544346)
            throw new InvalidDataException("Not a chara timeline (.tlb) file. Magic did not match.");

        uint version = bs.ReadUInt32();
        bs.Position += 0x10;
        int timelineDataOffset = bs.ReadInt32();

        bs.Position = timelineDataOffset;
        ReadTimelineData(bs);
    }

    private void ReadTimelineData(BinaryStream bs)
    {
        long thisPos = bs.Position;

        uint field_0x00 = bs.ReadUInt32();
        int timelineElementsOffset = bs.ReadInt32();
        uint timelineElementCount = bs.ReadUInt32();
        int assetGroupsOffset = bs.ReadInt32();
        uint assetGroupCount = bs.ReadUInt32();
        int offset_0x14 = bs.ReadInt32();
        uint count_0x14 = bs.ReadUInt32();
        TotalFrames = bs.ReadUInt32();
        bs.ReadUInt32();

        for (int i = 0; i < timelineElementCount; i++)
        {
            bs.Position = thisPos + timelineElementsOffset + (i * 0x20);
            var elem = new TimelineElement();
            elem.Read(bs);
            Elements.Add(elem);
        }
    }
}
