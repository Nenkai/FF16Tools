using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Assets;

public class AssetRegistry
{
    public List<UIAssetReference> TextureAssets { get; set; } = [];
    public List<UIAssetReference> UIAssets { get; set; } = [];
    public List<UIAssetReference> VFXAssets { get; set; } = [];

    public void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        uint textureAssetsOffset = bs.ReadUInt32();
        uint textureAssetCount = bs.ReadUInt32();
        uint uiAssetsOffset = bs.ReadUInt32();
        uint uiAssetsCount = bs.ReadUInt32();
        uint vfxAssetsOffset = bs.ReadUInt32();
        uint vfxAssetCount = bs.ReadUInt32();
        bs.ReadCheckPadding(0x1C);

        TextureAssets = bs.ReadStructsFromOffsetTable32<UIAssetReference>(basePos + textureAssetsOffset, textureAssetCount);
        UIAssets = bs.ReadStructsFromOffsetTable32<UIAssetReference>(basePos + uiAssetsOffset, uiAssetsCount);
        VFXAssets = bs.ReadStructsFromOffsetTable32<UIAssetReference>(basePos + vfxAssetsOffset, vfxAssetCount);
    }
}
