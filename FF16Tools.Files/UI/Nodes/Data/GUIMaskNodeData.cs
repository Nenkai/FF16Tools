using FF16Tools.Files.UI.Assets;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes.Data;

public class GUIMaskNodeData : GUINodeDataBase
{
    public List<AssetReference> Textures { get; set; } = [];

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        base.Read(bs);
        int textureAssetsOffset = bs.ReadInt32();
        uint textureAssetCount = bs.ReadUInt32();
        bs.ReadCheckPadding(0x40);

        bs.Position = basePos + textureAssetsOffset;
        Textures = bs.ReadArrayOfStructs<AssetReference>(textureAssetCount);
    }
}
