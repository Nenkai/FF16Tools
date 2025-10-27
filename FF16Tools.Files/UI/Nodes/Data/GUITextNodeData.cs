using FF16Tools.Files.UI.Assets;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes.Data;

public class GUITextNodeData : GUINodeDataBase
{
    public int UIKeyId { get; set; }
    public string UnkString_0x44 { get; set; }
    public int FontSize { get; set; }
    public byte Field_0x4C { get; set; }
    public byte Field_0x4D { get; set; }
    public byte Field_0x4E { get; set; }
    public int Spacing { get; set; }
    public int LineHeight { get; set; }
    public int VerticalAlignment { get; set; }
    public int HorizontalAlignment { get; set; }
    public int UnkType_0x60 { get; set; }
    public int Field_0x70 { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        base.Read(bs);
        UIKeyId = bs.ReadInt32();
        UnkString_0x44 = bs.ReadStringPointer(basePos);
        FontSize = bs.ReadInt32();
        Field_0x4C = bs.Read1Byte();
        Field_0x4D = bs.Read1Byte();
        Field_0x4E = bs.Read1Byte();
        bs.ReadCheckPadding(0x01);
        Spacing = bs.ReadInt32();
        LineHeight = bs.ReadInt32();
        VerticalAlignment = bs.ReadInt32();
        HorizontalAlignment = bs.ReadInt32();
        UnkType_0x60 = bs.ReadInt32();
        bs.ReadCheckPadding(0x0C);
        Field_0x70 = bs.ReadInt32();
        bs.ReadCheckPadding(0x0C);
    }
}
