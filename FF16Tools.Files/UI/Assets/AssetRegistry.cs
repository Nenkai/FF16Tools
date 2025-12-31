using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Assets;

public class AssetRegistry : ISerializableStruct
{
    public List<UIAssetReference> TextureAssets { get; set; } = [];
    public List<UIAssetReference> UIAssets { get; set; } = [];
    public List<UIAssetReference> VFXAssets { get; set; } = [];

    public uint GetSize()
    {
        return 0x34;
    }

    public void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        TextureAssets = bs.ReadStructArrayFromOffsetCountToOffsetTable32<UIAssetReference>(basePos);
        UIAssets = bs.ReadStructArrayFromOffsetCountToOffsetTable32<UIAssetReference>(basePos);
        VFXAssets = bs.ReadStructArrayFromOffsetCountToOffsetTable32<UIAssetReference>(basePos);
        bs.ReadCheckPadding(0x1C);
    }

    public void Write(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        long lastDataOffset = basePos + GetSize();
        bs.WriteStructArrayPointerWithOffsetTable32(basePos, TextureAssets, ref lastDataOffset);
        bs.WriteStructArrayPointerWithOffsetTable32(basePos, UIAssets, ref lastDataOffset);
        bs.WriteStructArrayPointerWithOffsetTable32(basePos, VFXAssets, ref lastDataOffset);
        bs.WritePadding(0x1C);

        bs.Position = lastDataOffset;
    }
}
