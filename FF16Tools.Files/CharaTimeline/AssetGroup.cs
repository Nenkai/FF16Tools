using Syroot.BinaryData;

namespace FF16Tools.Files.CharaTimeline
{
    public class Asset : BaseStruct
    {
        public override int _totalSize => 0x1C;

        public int field_0x00;
        public int field_0x04;
        public int field_0x08;
        public int field_0x0C;
        public int field_0x10;
        public int field_0x14;
        public int field_0x18;

        [RelativeField("field_0x14")]
        public string FileName;

        [RelativeField("field_0x18")]
        public string Path;
    }

    public class AssetGroup : BaseStruct
    {
        public override int _totalSize => 0xC;

        public int Index;
        // In the asset groups after the 3rd one (which actually has assets), this value is some weird large number
        // During write this is "fixed" to actually point to the array list, either way this doesn't seem to matter
        public int AssetArrayListOffset; 
        public int NumAssets;

        [RelativeField("AssetArrayListOffset")]
        public int[] AssetEntryOffsets;

        [RelativeField("AssetArrayListOffset")]
        public List<Asset> AssetList;

        public override void Read(BinaryStream bs) { 
            long startOffset = bs.Position;

            Index = bs.ReadInt32();
            AssetArrayListOffset = bs.ReadInt32();
            NumAssets = bs.ReadInt32();

            long finalPos = bs.Position;

            AssetEntryOffsets = new int[NumAssets];
            bs.Position = startOffset + AssetArrayListOffset;
            for (int i = 0; i < NumAssets; i++)
            {
                AssetEntryOffsets[i] = bs.ReadInt32();
            }

            
            if (NumAssets > 0) {
                AssetList = Timeline.ReadArrayOfStructs<Asset>(bs, startOffset + AssetArrayListOffset + AssetEntryOffsets[0], NumAssets);
            }
            bs.Position = finalPos;
        }

        public override void Write(BinaryStream bs, Dictionary<(object, string), long> relativeFieldPos, Dictionary<string, long> stringPos)
        {
            long AssetEntryOffsetsPosition = relativeFieldPos[(this, "AssetEntryOffsets")];
            int offsetToEntryList = (int)(AssetEntryOffsetsPosition - bs.Position);
            bs.WriteInt32(Index);
            bs.WriteInt32(offsetToEntryList);
            bs.WriteInt32(AssetList != null ? AssetList.Count : 0);
        }
    }
}
