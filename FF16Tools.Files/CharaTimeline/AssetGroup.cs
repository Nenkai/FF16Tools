using Syroot.BinaryData;

namespace FF16Tools.Files.CharaTimeline
{
    public class Asset : BaseStruct
    {
        public override int _totalSize => 0x1C;

        int field_0x00;
        int field_0x04;
        int field_0x08;
        int field_0x0C;
        int field_0x10;

        [OffsetAttribute("FileName")]
        int field_0x14;
        [OffsetAttribute("Path")]
        int field_0x18;
    }

    public class AssetGroup : BaseStruct
    {
        public override int _totalSize => 0xC;

        int Index;
        [OffsetAttribute("AssetList")]
        int AssetArrayListOffset;
        int NumAssets;

        public override void Read(BinaryStream bs) { 
            long startOffset = bs.Position;

            Index = bs.ReadInt32();
            AssetArrayListOffset = bs.ReadInt32();
            NumAssets = bs.ReadInt32();

            long finalPos = bs.Position;
            if (NumAssets > 0) {
                bs.Position = startOffset + AssetArrayListOffset;
                _referencedArrays["AssetList"] = Timeline.ReadArrayOfStructs<Asset>(bs, bs.Position + bs.ReadInt32(), NumAssets).Select(s => (BaseStruct)s).ToList();
            }
            bs.Position = finalPos;
        }
    }
}
