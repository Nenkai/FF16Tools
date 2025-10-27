using FF16Tools.Files.UI.Assets;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes.Data;

public class GUIImageNodeData : GUINodeDataBase
{
    public int Field_0x68 { get; set; }
    public int Field_0x6C { get; set; }
    public int Field_0x70 { get; set; }
    public List<UIAssetReference> TextureAssetReferences { get; set; } = [];

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        base.Read(bs);
        uint textureAssetsOffset = bs.ReadUInt32();
        uint textureAssetCount = bs.ReadUInt32();
        bs.ReadCheckPadding(0x20);

        Field_0x68 = bs.ReadInt32();
        Field_0x6C = bs.ReadInt32();
        Field_0x70 = bs.ReadInt32();
        bs.ReadCheckPadding(0x20);

        TextureAssetReferences = bs.ReadStructsFromOffsetTable32<UIAssetReference>(basePos + textureAssetsOffset, textureAssetCount);
    }
}
