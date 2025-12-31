using Syroot.BinaryData;

namespace FF16Tools.Files.Timelines;

public class AssetGroup : ISerializableStruct
{
    public int Index { get; set; }
    public List<Asset> AssetList { get; set; } = [];

    public void Read(SmartBinaryStream bs)
    {
        long startOffset = bs.Position;

        Index = bs.ReadInt32();
        AssetList = bs.ReadStructArrayFromOffsetCountToOffsetTable32<Asset>(startOffset);
    }

    public void Write(SmartBinaryStream bs)
    {
        long basePos = bs.Position;
        long lastDataPos = bs.GetMarker().LastDataPosition;

        bs.WriteInt32(Index);
        bs.WriteStructArrayPointerWithOffsetTable32(basePos, AssetList, ref lastDataPos);

        bs.Position = lastDataPos;
    }

    public uint GetSize() => 0x0C;
}
