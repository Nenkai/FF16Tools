using FF16Tools.Files.UI.Assets;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes.Data;

public class GUINinegridNodeData : GUINodeDataBase
{
    public List<AssetReference> Textures { get; set; } = [];
    public int Field_0x68 { get; set; }
    public int Field_0x6C { get; set; }
    public int Field_0x70 { get; set; }
    public int Field_0x74 { get; set; }
    public int Field_0x78 { get; set; }
    public int Field_0x7C { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        base.Read(bs);
        int textureAssetsOffset = bs.ReadInt32();
        uint textureAssetCount = bs.ReadUInt32();
        bs.ReadCheckPadding(0x20);
        Field_0x68 = bs.ReadInt32();
        Field_0x6C = bs.ReadInt32();
        Field_0x70 = bs.ReadInt32();
        Field_0x74 = bs.ReadInt32();
        Field_0x78 = bs.ReadInt32();
        Field_0x7C = bs.ReadInt32();
        bs.ReadCheckPadding(0x08);

        bs.Position = basePos + textureAssetsOffset;
        Textures = bs.ReadArrayOfStructs<AssetReference>(textureAssetCount);
    }
}
