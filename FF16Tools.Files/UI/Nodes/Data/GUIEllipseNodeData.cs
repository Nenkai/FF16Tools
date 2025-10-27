using FF16Tools.Files.UI.Assets;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes.Data;

public class GUIEllipseNodeData : GUINodeDataBase
{
    public int Field_0x40 { get; set; }
    public float Field_0x44 { get; set; }
    public int Field_0x48 { get; set; }
    public int Field_0x4C { get; set; }
    public List<UIAssetReference> TextureAssetReferences { get; set; } = [];

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        base.Read(bs);
        Field_0x40 = bs.ReadInt32();
        Field_0x44 = bs.ReadSingle();
        Field_0x48 = bs.ReadInt32();
        Field_0x4C = bs.ReadInt32();
        uint textureAssetsOffset = bs.ReadUInt32();
        uint textureAssetCount = bs.ReadUInt32();
        TextureAssetReferences = bs.ReadStructsFromOffsetTable32<UIAssetReference>(basePos + textureAssetsOffset, textureAssetCount);
    }
}
