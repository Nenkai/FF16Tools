using FF16Tools.Files.Timelines.Chara.Targets;
using FF16Tools.Files.Timelines.Elements.Battle;
using FF16Tools.Files.Timelines.UI.Targets;

namespace FF16Tools.Files.Timelines;

public class Timeline : ISerializableStruct
{
    /// <summary>
    /// Version is expected to be 3.
    /// </summary>
    public uint TimelineVersion { get; set; }

    /// <summary>
    /// Last frame index in the timeline. There are 30 frames per second.
    /// </summary>
    public uint LastFrameIndex { get; set; }
    public List<TimelineElement> Elements { get; set; } = [];
    public List<AssetGroup> AssetGroups { get; set; } = [];
    public List<TimelineTargetBase> Targets { get; set; } = [];
    public int Field_0x20 { get; set; }

    public uint GetSize() => 0x24;

    public void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        TimelineVersion = bs.ReadUInt32();
        Elements = bs.ReadStructArrayFromOffsetCount<TimelineElement>(basePos);
        AssetGroups = bs.ReadStructArrayFromOffsetCount<AssetGroup>(basePos);
        int targetsOffset = bs.ReadInt32();
        int targetsCount = bs.ReadInt32();

        LastFrameIndex = bs.ReadUInt32();
        Field_0x20 = bs.ReadInt32();

        long tempPos = bs.Position;
        for (int i = 0; i < targetsCount; i++)
        {
            bs.Position = basePos + targetsOffset + (i * 0x04);
            int timelineTargetOffset = bs.ReadInt32();

            bs.Position = basePos + targetsOffset + timelineTargetOffset;
            uint targetType = bs.ReadUInt32();
            bs.Position -= 4;

            TimelineTargetBase target = targetType switch
            {
                1001 => new CharaTarget1001(),
                4001 => new UITimelineComponentTarget(),
                4002 => new UITimelineTarget4002(),
                _ => throw new NotImplementedException($"Timeline target type {targetType} invalid or not implemented")
            };
            target.Read(bs);
            Targets.Add(target);
        }
        bs.Position = tempPos;
    }

    /// <summary>
    /// Gets the duration of the timeline in frames.
    /// </summary>
    /// <returns></returns>
    public uint CalculateTimelineFrameLength()
    {
        var elem = Elements.MaxBy(e => e.FrameStart + e.NumFrames);
        if (elem is null)
            return 0;

        return elem.FrameStart + elem.NumFrames;
    }

    public void Write(SmartBinaryStream bs)
    {
        long basePos = bs.Position;
        long lastDataPos = bs.GetMarker().LastDataPosition;

        // We aim to write the data 1:1.

        // We should be writing the element infos here. For now, skip them.
        // We need the data offsets of other timeline elements.
        bs.WriteUInt32(TimelineVersion);
        bs.WriteStructArrayPointer(basePos, Elements, ref lastDataPos);
        bs.WriteStructArrayPointer(basePos, AssetGroups, ref lastDataPos);
        bs.WriteStructArrayPointerWithOffsetTable32(basePos, Targets, ref lastDataPos);
        bs.WriteUInt32(LastFrameIndex);
        bs.WriteInt32(Field_0x20);

        bs.GetMarker().LastDataPosition = lastDataPos;
    }

    /// <summary>
    /// Checks that all elements are within the frame range of the timeline or throws if not.
    /// </summary>
    /// <exception cref="InvalidDataException"></exception>
    public void CheckTimelineElementFrameRanges()
    {
        for (int i = 0; i < Elements.Count; i++)
        {
            TimelineElement element = Elements[i];
            uint startFrame = element.FrameStart;
            uint endFrame = element.FrameStart + element.NumFrames;
            if (startFrame > LastFrameIndex || endFrame > LastFrameIndex)
                throw new InvalidDataException($"Timeline element index {i} is out of frame range of the timeline (timeline frames: {LastFrameIndex}, element: {startFrame}->{endFrame}");
        }
    }

    // These timeline elements are written before the timeline header
    public static readonly HashSet<TimelineElementType> _preTimelineTypes =
    [
        TimelineElementType.kTimelineElem_30,
        TimelineElementType.PlaySoundTrigger,
        TimelineElementType.ModelSE,
        (TimelineElementType)1029,
        (TimelineElementType)1042,
    ];

    public void WritePreTimelineElements(SmartBinaryStream bs)
    {
        bool aligned = false;
        for (int i = 0; i < Elements.Count; i++)
        {
            TimelineElement element = Elements[i];
            if (element.DataUnion is null)
                throw new InvalidOperationException($"Data for timeline element at index {i} is null, cannot write element.");

            if (_preTimelineTypes.Contains(element.DataUnion.UnionType))
            {
                if (!aligned)
                {
                    bs.Align(0x10, grow: true);
                    aligned = true;
                }

                bs.AddObjectPointer(element.DataUnion);

                // We don't write them right away. Why, you ask?
                // String table order. It seems the strings are correctly ordered according to the order of elements
                // And even though this is written before the timeline header, the strings are still in the right order.
                // So we can't write the data early just yet. We pre-allocate pretty much.
                bs.Position += ((ISerializableStruct)element.DataUnion).GetSize();
            }

            // These sub-structures are also written before. These ones we can actually write, they don't have strings so order doesn't matter.
            switch (element.DataUnion.UnionType)
            {
                // TODO: Shorten this
                case TimelineElementType.kTimelineElem_1023:
                    {
                        var elem = (TimelineElement_1023)element.DataUnion;

                        // Aligned regardless if count is 0
                        if (!aligned)
                        {
                            bs.Align(0x10, grow: true);
                            aligned = true;
                        }

                        bs.AddObjectPointer(elem.SubStructs);
                        for (int j = 0; j < elem.SubStructs.Count; j++)
                            elem.SubStructs[j].Write(bs);
                    }
                    break;

                case TimelineElementType.kTimelineElem_1030:
                    {
                        var elem = (TimelineElement_1030)element.DataUnion;

                        // Aligned regardless if count is 0
                        if (!aligned)
                        {
                            bs.Align(0x10, grow: true);
                            aligned = true;
                        }

                        bs.AddObjectPointer(elem.SubStructs);
                        for (int j = 0; j < elem.SubStructs.Count; j++)
                            elem.SubStructs[j].Write(bs);
                    }
                    break;

                case TimelineElementType.kTimelineElem_1049:
                    {
                        var elem = (TimelineElement_1049)element.DataUnion;

                        // Aligned regardless if count is 0
                        if (!aligned)
                        {
                            bs.Align(0x10, grow: true);
                            aligned = true;
                        }

                        bs.AddObjectPointer(elem.SubStructs);
                        for (int j = 0; j < elem.SubStructs.Count; j++)
                            elem.SubStructs[j].Write(bs);
                    }
                    break;
                case TimelineElementType.kTimelineElem_1050:
                    {
                        var elem = (TimelineElement_1050)element.DataUnion;

                        // Aligned regardless if count is 0
                        if (!aligned)
                        {
                            bs.Align(0x10, grow: true);
                            aligned = true;
                        }

                        bs.AddObjectPointer(elem.SubStructs);
                        for (int j = 0; j < elem.SubStructs.Count; j++)
                            elem.SubStructs[j].Write(bs);
                    }
                    break;
            }
        }
    }
}

public enum TimelineType : uint
{
    UI = 3,
}
