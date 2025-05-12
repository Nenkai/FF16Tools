using Syroot.BinaryData;

namespace FF16Tools.Files.Timelines.Chara
{
    public class Timeline
    {
        public int TotalFrames { get; set; }
        public List<TimelineElement> Elements { get; set; }
        public List<AssetGroup> AssetGroups { get; set; }
        public List<FinalStruct> FinalStructs { get; set; }

        int field_0x00;
        int timelineElementsOffset;

        public void Read(BinaryStream bs)
        {
            long thisPos = bs.Position;

            field_0x00 = bs.ReadInt32();
            timelineElementsOffset = bs.ReadInt32();
            int timelineElementCount = bs.ReadInt32();
            int assetGroupsOffset = bs.ReadInt32();
            int assetGroupCount = bs.ReadInt32();
            int offset_0x14 = bs.ReadInt32();
            int count_0x14 = bs.ReadInt32();
            int TotalFrames = bs.ReadInt32();
            bs.ReadInt32(); // empty padding

            Elements = ReadArrayOfStructs<TimelineElement>(bs, thisPos + timelineElementsOffset, timelineElementCount);
            AssetGroups = ReadArrayOfStructs<AssetGroup>(bs, thisPos + assetGroupsOffset, assetGroupCount);
            FinalStructs = ReadArrayOfStructs<FinalStruct>(bs, thisPos + offset_0x14, count_0x14);
        }
        public static List<T> ReadArrayOfStructs<T>(BinaryStream bs, long startOffset, int elementCount) where T : BaseStruct, new()
        {
            List<T> elements = new();

            for (int i = 0; i < elementCount; i++)
            {
                T elem = new T();
                bs.Position = startOffset + i * elem.TotalSize;
                elem.Read(bs);
                elements.Add(elem);
            }

            return elements;
        }

        public List<string> GetAllDistinctStrings()
        {
            return Elements.SelectMany(e => e.GetAllStrings()).Concat(
                AssetGroups.Where(a => a.AssetList != null).SelectMany(a => a.AssetList).SelectMany(a => a.GetAllStrings())
            ).Distinct().ToList();
        }

        public void Write(BinaryStream bs)
        {
            HashSet<int> unknownElements = Elements.Where(e=>e.DataUnion.ElementData == null).Select(e => e.DataUnion.UnionType).ToHashSet();
            if (unknownElements.Count > 0) { 
                throw new Exception($"The timeline cannot be written since it contains unknow union types: ({string.Join(", ", unknownElements)})");
            }

            // We first need to write the absolute timeline position, then 4 empty bytes
            // Then we need to write the "pre-timeline" data, which is any DataUnion of type {30,31,45,1029,1042},
            // Or any list of structs that are referenced by the timeline elements.
            // To do this we need to calculate a bunch of data sizes and offsets, and write them in the correct order.

            // Holds a mapping between each relative object and its position in the file (key is the owning object and the field name)
            Dictionary<(object, string), long> relativeFieldPos = new();

            HashSet<int> preTimelineTypes = new() { 30, 31, 45, 1029, 1042 };
            List<TimelineElement> preTimelineElements = new();
            List<TimelineElement> postTimelineElements = new();

            foreach (var elem in Elements)
            {
                if (preTimelineTypes.Contains(elem.DataUnion.UnionType))
                    preTimelineElements.Add(elem);
                else
                    postTimelineElements.Add(elem);
            }

            List<RelativeListInfo> preTimelineLists = Elements.SelectMany(e=>e.DataUnion.ElementData.GetAllRelativeLists()).ToList();

            // Figure out the size of the pre-timeline data to calculate where the timeline will be
            int preTimelineDataUnionSize = preTimelineElements.Select(e=>e.DataUnion.GetNonRelativeSize()).Sum();
            int preTimelineListsSize = preTimelineLists.SelectMany(f=>f.value).Select(l => l.GetNonRelativeSize()).Sum();

            // If there is pre-timeline data, there is a 4 byte empty padding after the header
            int headerPaddingSize = preTimelineDataUnionSize + preTimelineListsSize == 0 ? 0 : 4; 

            // 4 bytes for the timeline position itself
            int timelinePos = (int)bs.Position + 4 + headerPaddingSize + preTimelineListsSize + preTimelineDataUnionSize;
            bs.Write(timelinePos);
            if (headerPaddingSize > 0)
                bs.WriteInt32(0); // empty padding

            // ***** End of header *****

            // Offset calculations:

            // Calculate AssetGroup offset, which is at the position of metadata + all elements + all post-timeline data unions
            var elementsSize = Elements.Select(e => e.GetNonRelativeSize()).Sum();
            // Ignore the size of the elements that came before the timeline
            var dataUnionSize = postTimelineElements.Select(e => e.DataUnion.GetNonRelativeSize()).Sum();

            var assetGroupOffset = timelineElementsOffset + elementsSize + dataUnionSize;

            // Calculate FinalStruct offset, which is at the position of metadata + all elements + all data unions + all asset groups + all asset
            var assetGroupSize = AssetGroups.Select(a => a.GetNonRelativeSize()).Sum();
            var assetEntryOffsetsSize = AssetGroups.Where(a=>a.AssetList != null).Select(a => a.AssetList.Count).Sum() * 4; // 4 since its a list of ints
            var assetsSize = AssetGroups.Where(a=>a.AssetList != null).SelectMany(a => a.AssetList).Select(a => a.GetNonRelativeSize()).Sum();
            var finalStructOffset = assetGroupOffset + assetGroupSize + assetEntryOffsetsSize + assetsSize;

            // Calculate strings absolute position, which is after everything else
            var finalStructsSize = FinalStructs.Select(f => f.GetNonRelativeSize()).Sum();
            var finalStructSubStructsSize = FinalStructs.Select(f => f.Sub.GetNonRelativeSize()).Sum();
            var stringsStartingPos = timelinePos + finalStructOffset + finalStructsSize + finalStructSubStructsSize;

            // Write all strings to a temp buffer, and save their would-be position
            using MemoryStream stringBuffer = new();
            List<string> distStrings = GetAllDistinctStrings();
            Dictionary<string, long> stringPos = new();
            foreach (var str in distStrings)
            {
                stringPos[str] = stringBuffer.Position + stringsStartingPos;
                stringBuffer.WriteString(str, StringCoding.ZeroTerminated, encoding: System.Text.Encoding.UTF8);
            }


            // ** Write pre-timeline data **
            // Write all relative lists and save their positions
            foreach (var lst in preTimelineLists)
            {
                relativeFieldPos[(lst.OwningObject, lst.fieldName)] = bs.Position;
                foreach (var item in lst.value)
                {
                    // The relative lists dont refer to other internal relative elements
                    item.Write(bs, relativeFieldPos: null, stringPos: stringPos);
                }
            }

            // Write all pre-timeline data unions and save their positions
            foreach (var elem in preTimelineElements)
            {
                relativeFieldPos[(elem, "DataUnion")] = bs.Position;
                elem.DataUnion.Write(bs, relativeFieldPos: relativeFieldPos, stringPos: stringPos);
            }

            // ***** End of pre-timeline data ****

            // ** Write timeline **

            bs.WriteInt32(field_0x00);
            bs.WriteInt32(timelineElementsOffset); // always 36
            bs.WriteInt32(Elements.Count);
            bs.WriteInt32(assetGroupOffset); // assetGroupsOffset
            bs.WriteInt32(AssetGroups.Count);
            bs.WriteInt32(finalStructOffset); // offset_0x14
            bs.WriteInt32(FinalStructs.Count);
            bs.WriteInt32(Elements.Select(e => e.FrameStart + e.NumFrames).Max()); // TotalFrames
            bs.WriteInt32(0); // empty padding

            // Calculate the timeline elements positions without actually writing them, since they are needed when writing the elements
            var elemDataUnionPos = bs.Position + elementsSize;
            foreach (var elem in postTimelineElements) {
                relativeFieldPos[(elem, "DataUnion")] = elemDataUnionPos;
                elemDataUnionPos += elem.DataUnion.GetNonRelativeSize();
            }

            // Now actually write the elements
            foreach (var elem in Elements)
            {
                elem.Write(bs, relativeFieldPos: relativeFieldPos, stringPos: stringPos);
            }

            // Write all data unions
            foreach (var elem in postTimelineElements)
            {
                elem.DataUnion.Write(bs, relativeFieldPos: relativeFieldPos, stringPos: stringPos);
            }

            // ***** End of data elements *****

            // ** Write asset groups **

            // Calculate the positions of the relative asset stuff without actually writing them, since they are needed when writing the assets
            var assetEntryOffsetPos = bs.Position + assetGroupSize;
            // The asset entry offset, and all the assets are only ever written in the 3rd AssetGroup, but i'll leave it dynamic just in case
            var assetEntriesListPos = assetEntryOffsetPos + assetEntryOffsetsSize;
            foreach (var assetGroup in AssetGroups)
            {
                relativeFieldPos[(assetGroup, "AssetEntryOffsets")] = assetEntryOffsetPos;
                relativeFieldPos[(assetGroup, "AssetList")] = assetEntryOffsetPos + assetEntryOffsetsSize;
                assetGroup.Write(bs, relativeFieldPos: relativeFieldPos, stringPos: stringPos);
            }
            // Write the asset entry offsets
            List<Asset> allAssets = AssetGroups.Where(ag=>ag.AssetList != null).SelectMany(ag => ag.AssetList).ToList();
            for (int i = 0; i<allAssets.Count; i++) {
                var offsetToAsset = assetEntryOffsetsSize + allAssets[0].TotalSize * i;
                bs.WriteInt32(offsetToAsset);
            }
            // Write the assets
            foreach (Asset asset in allAssets)
            {
                asset.Write(bs, relativeFieldPos: relativeFieldPos, stringPos: stringPos);
            }

            // ***** End of asset groups *****

            // ** Write final structs **
            long finalSubStructPos = bs.Position + finalStructsSize;
            int finalStructIndex = 0;
            foreach (var finalStruct in FinalStructs)
            {
                relativeFieldPos[(finalStruct, "Sub")] = finalSubStructPos + FinalStructs[0].Sub.TotalSize * finalStructIndex;
                finalStruct.Write(bs, relativeFieldPos: relativeFieldPos, stringPos: stringPos);
                finalStructIndex++;
            }

            // Write the sub structs
            foreach (var finalStruct in FinalStructs)
            {
                finalStruct.Sub.Write(bs, relativeFieldPos: relativeFieldPos, stringPos: stringPos);
            }

            // ***** End of final structs *****

            // ** Write strings **
            stringBuffer.Position = 0;
            stringBuffer.CopyTo(bs);
            // ***** End of strings *****
        }
    }
}
