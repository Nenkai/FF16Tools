using FF16Tools.Files.UI.Assets;
using FF16Tools.Files.UI.Timelines.Targets;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Timelines;

public class UITimelineInfo
{
    public uint TypeOrVersion { get; set; }

    public List<UITimelineTargetBase> Targets { get; set; } = [];
    public uint FrameCount { get; set; }
    public uint Field_0x28 { get; set; }

    public void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        TypeOrVersion = bs.ReadUInt32();
        bs.Position += 8; // bs.ReadStructArrayFromOffsetCount<UITimeline>(basePos);
        bs.Position += 8;
        int timelineTargetsOffset = bs.ReadInt32();
        uint timelineTargetCount = bs.ReadUInt32();
        FrameCount = bs.ReadUInt32();
        Field_0x28 = bs.ReadUInt32();

        long tempPos = bs.Position;
        for (int i = 0; i < timelineTargetCount; i++)
        {
            bs.Position = basePos + timelineTargetsOffset + (i * 0x04);
            int timelineTargetOffset = bs.ReadInt32();

            bs.Position = basePos + timelineTargetsOffset + timelineTargetOffset;
            uint targetType = bs.ReadUInt32();
            bs.Position -= 4;

            UITimelineTargetBase target = targetType switch
            {
                4001 => new UITimelineComponentTarget(),
                4002 => new UITimelineTarget4002(),
                _ => throw new NotImplementedException($"Timeline target type {targetType} invalid or not implemented")
            };
            target.Read(bs);
            Targets.Add(target);
        }
        bs.Position = tempPos;
    }

    public void Write(SmartBinaryStream bs)
    {
        throw new NotImplementedException();
    }
}
