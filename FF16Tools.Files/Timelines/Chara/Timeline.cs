using FF16Tools.Files.Timelines.Elements.Battle;

using Syroot.BinaryData;

using System.ComponentModel.DataAnnotations.Schema;

namespace FF16Tools.Files.Timelines.Chara;

public class Timeline
{
    public int Field_0x00 { get; set; }

    /// <summary>
    /// Last frame index in the timeline. There are 30 frames per second.
    /// </summary>
    public uint LastFrameIndex { get; set; }
    public List<TimelineElement> Elements { get; set; } = [];
    public List<AssetGroup> AssetGroups { get; set; } = [];
    public List<SetupData> SetupData { get; set; } = [];
    public int Field_0x28 { get; set; }

    public void Read(SmartBinaryStream bs)
    {
        long thisPos = bs.Position;

        Field_0x00 = bs.ReadInt32();
        int timelineElementsOffset = bs.ReadInt32();
        uint timelineElementCount = bs.ReadUInt32();
        int assetGroupsOffset = bs.ReadInt32();
        uint assetGroupCount = bs.ReadUInt32();
        int setupDatasOffset = bs.ReadInt32();
        uint setupDataCount = bs.ReadUInt32();
        LastFrameIndex = bs.ReadUInt32();
        Field_0x28 = bs.ReadInt32();

        Elements = bs.ReadStructArray<TimelineElement>(thisPos + timelineElementsOffset, timelineElementCount);
        AssetGroups = bs.ReadStructArray<AssetGroup>(thisPos + assetGroupsOffset, assetGroupCount);
        SetupData = bs.ReadStructsFromOffsetTable32<SetupData>(thisPos + setupDatasOffset, setupDataCount);
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

    public int Write(SmartBinaryStream bs, CharaTimelineSerializationOptions serializationOptions)
    {
        if (serializationOptions.CalculateTotalFrameCount)
            LastFrameIndex = CalculateTimelineFrameLength();
        else
            CheckTimelineElementFrameRanges();

        // We aim to write the data 1:1.

        // The timeline header can appear after some element data, which is weird, I wonder why they do this.
        // Write said data first.
        WritePreTimelineElements(bs);

        long timelineHeaderOffset = bs.Position;
        bs.Position += 0x24;

        // We should be writing the element infos here. For now, skip them.
        // We need the data offsets of other timeline elements.
        long timelineElementsOffset = bs.Position;
        WriteTimelineElements(bs);

        // Done with timeline elements. Write asset groups.
        long assetGroupsOffset = bs.Position;
        WriteAssetGroups(bs);

        // Write whatever this is.
        long finalStructsOffset = bs.Position;
        WriteSetupDataList(bs);

        // String table.
        bs.WriteStringTable();

        long endPos = bs.Position;

        // Finalize timeline header
        bs.Position = timelineHeaderOffset;
        bs.WriteInt32(Field_0x00);
        bs.WriteInt32((int)(timelineElementsOffset - timelineHeaderOffset));
        bs.WriteInt32(Elements.Count);
        bs.WriteInt32((int)(assetGroupsOffset - timelineHeaderOffset));
        bs.WriteInt32(AssetGroups.Count);
        bs.WriteInt32((int)(finalStructsOffset - timelineHeaderOffset));
        bs.WriteInt32(SetupData.Count);
        bs.WriteUInt32(LastFrameIndex);
        bs.WriteInt32(Field_0x28);

        bs.Position = endPos;

        return (int)timelineHeaderOffset;
    }

    /// <summary>
    /// Checks that all elements are within the frame range of the timeline or throws if not.
    /// </summary>
    /// <exception cref="InvalidDataException"></exception>
    private void CheckTimelineElementFrameRanges()
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

    private void WriteSetupDataList(SmartBinaryStream bs)
    {
        // Skip toc for now.
        long setupDataToc = bs.Position;
        bs.Position += SetupData.Count * sizeof(int);

        // Got the start data offset to setup structs. Begin to write
        for (int i = 0; i < SetupData.Count; i++)
        {
            SetupData setupData = SetupData[i];
            bs.AddObjectPointer(setupData);
            setupData.Write(bs);
        }
        long lastOffset = bs.Position;

        // Write setup data pointers.
        bs.Position = setupDataToc;
        for (int i = 0; i < SetupData.Count; i++)
        {
            long setupDataOffset = bs.Position;

            SetupData setupData = SetupData[i];
            bs.WriteObjectPointer(setupData, setupDataOffset);
        }

        // Done.
        bs.Position = lastOffset;
    }

    private void WriteTimelineElements(SmartBinaryStream bs)
    {
        long elementsInfoOffset = bs.Position;
        bs.Position += Elements.Count * 0x20;

        // Write all other elements.
        WriteMainTimelineElementData(bs);

        long lastOffset = bs.Position;
        bs.Position = elementsInfoOffset;

        // Now we can write the element toc.
        for (int i = 0; i < Elements.Count; i++)
            Elements[i].Write(bs);

        bs.Position = lastOffset;
    }

    // These timeline elements are written before the timeline header
    private readonly HashSet<TimelineElementType> _preTimelineTypes =
    [
        TimelineElementType.kTimelineElem_30,
        TimelineElementType.PlaySoundTrigger,
        TimelineElementType.ModelSE,
        (TimelineElementType)1029,
        (TimelineElementType)1042,
    ];

    private void WritePreTimelineElements(SmartBinaryStream bs)
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

    private void WriteMainTimelineElementData(SmartBinaryStream bs)
    {
        foreach (var element in Elements)
        {
            // Won't be null, already checked in WritePreTimelineElements.
            if (_preTimelineTypes.Contains(element.DataUnion!.UnionType))
            {
                long tmpPos = bs.Position;

                // See comment in WritePreTimelineElements(). We actually write the data here.
                long elementOffset = bs.GetObjectPointerOffset(element.DataUnion);
                bs.Position = elementOffset;
                element.DataUnion.Write(bs);

                bs.Position = tmpPos;
            }
            else
            {
                bs.AddObjectPointer(element.DataUnion);
                element.DataUnion.Write(bs);
            }
        }
    }

    private void WriteAssetGroups(SmartBinaryStream bs)
    {
        // Skip toc for now.
        long assetGroupToc = bs.Position;
        bs.Position += AssetGroups.Count * 0x0C;

        long assetOffsetListOffset = bs.Position;

        // Skip asset pointers aswell. We want the data offset.
        for (int i = 0; i < AssetGroups.Count; i++)
        {
            AssetGroup assetGroup = AssetGroups[i];
            bs.AddObjectPointer(assetGroup); // <- Make sure to keep track where the asset pointer list is, for groups.

            long assetTocOffset = bs.Position;
            bs.Position += assetGroup.AssetList.Count * sizeof(int); // Skip asset toc for now

            for (int j = 0; j < assetGroup.AssetList.Count; j++)
            {
                Asset asset = assetGroup.AssetList[j];
                bs.AddObjectPointer(asset);
                asset.Write(bs);
            }

            long lastPos = bs.Position;

            bs.Position = assetTocOffset;
            for (int j = 0; j < assetGroup.AssetList.Count; j++)
            {
                Asset asset = assetGroup.AssetList[j];
                bs.WriteObjectPointer(asset, assetOffsetListOffset);
            }

            bs.Position = lastPos;
        }

        long lastOffset = bs.Position;

        // Write asset group pointers.
        bs.Position = assetGroupToc;
        for (int i = 0; i < AssetGroups.Count; i++)
        {
            long assetGroupOffset = bs.Position;

            AssetGroup assetGroup = AssetGroups[i];
            bs.WriteInt32(assetGroup.Index);
            bs.WriteObjectPointer(assetGroup, assetGroupOffset);
            bs.WriteInt32(assetGroup.AssetList.Count);
        }

        // Done with asset groups.
        bs.Position = lastOffset;
    }
}
