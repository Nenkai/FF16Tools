using Syroot.BinaryData;

namespace FF16Tools.Files.CharaTimeline
{
    public class Timeline
    {
        public uint TotalFrames { get; set; }
        public List<TimelineElement> Elements;
        public List<AssetGroup> AssetGroups;
        public List<FinalStruct> FinalStructs;

        public void Read(BinaryStream bs) {
            long thisPos = bs.Position;

            int field_0x00 = bs.ReadInt32();
            int timelineElementsOffset = bs.ReadInt32();
            int timelineElementCount = bs.ReadInt32();
            int assetGroupsOffset = bs.ReadInt32();
            int assetGroupCount = bs.ReadInt32();
            int offset_0x14 = bs.ReadInt32();
            int count_0x14 = bs.ReadInt32();
            TotalFrames = bs.ReadUInt32();
            bs.ReadUInt32(); // empty padding

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
                bs.Position = startOffset + (i * elem._totalSize);
                elem.Read(bs);
                elements.Add(elem);
            }

            return elements;
        }
    }
}
