using Syroot.BinaryData;

namespace FF16Tools.Files.Timelines.Chara;

public class AssetGroup : ISerializableStruct
{
    public int Index { get; set; }
    public List<Asset> AssetList { get; set; } = [];

    public void Read(SmartBinaryStream bs)
    {
        long startOffset = bs.Position;

        Index = bs.ReadInt32();
        int assetArrayListOffset = bs.ReadInt32();
        int numAssets = bs.ReadInt32();

        long tempPos = bs.Position;

        long offsetListOffset = startOffset + assetArrayListOffset;
        for (int i = 0; i < numAssets; i++)
        {
            bs.Position = offsetListOffset + (i * sizeof(int));
            int assetOffset = bs.ReadInt32();

            bs.Position = offsetListOffset + assetOffset;
            Asset asset = new Asset();
            asset.Read(bs);

            AssetList.Add(asset);
        }

        bs.Position = tempPos;
    }

    public void Write(SmartBinaryStream bs)
    {
        throw new NotSupportedException("AssetGroups cannot be written separately");
    }

    public uint GetSize() => 0x0C;
}
