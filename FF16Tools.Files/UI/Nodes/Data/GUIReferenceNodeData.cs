using FF16Tools.Files.UI.Assets;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes.Data;

public class GUIReferenceNodeData : GUINodeDataBase
{
    public int Field_0x40 { get; set; }
    public int Field_0x44 { get; set; }
    public int Field_0x48 { get; set; }
    public int Field_0x4C { get; set; }
    public int Field_0x50 { get; set; }
    public int Field_0x54 { get; set; }
    public int Field_0x58 { get; set; }
    public int Field_0x5C { get; set; }
    public int Field_0x60 { get; set; }
    public int Field_0x64 { get; set; }
    public int Field_0x68 { get; set; }
    public int Field_0x6C { get; set; }
    public int Field_0x70 { get; set; }
    public int Field_0x74 { get; set; }
    public int Field_0x78 { get; set; }
    public int Field_0x7C { get; set; }
    public int Field_0x80 { get; set; }
    public int Field_0x84 { get; set; }
    public int Field_0x88 { get; set; }
    public int Field_0x8C { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        base.Read(bs);
        Field_0x40 = bs.ReadInt32();
        Field_0x44 = bs.ReadInt32();
        Field_0x48 = bs.ReadInt32();
        Field_0x4C = bs.ReadInt32();
        Field_0x50 = bs.ReadInt32();
        Field_0x54 = bs.ReadInt32();
        Field_0x58 = bs.ReadInt32();
        Field_0x5C = bs.ReadInt32();
        Field_0x60 = bs.ReadInt32();
        Field_0x64 = bs.ReadInt32();
        Field_0x68 = bs.ReadInt32();
        Field_0x6C = bs.ReadInt32();
        Field_0x70 = bs.ReadInt32();
        Field_0x74 = bs.ReadInt32();
        Field_0x78 = bs.ReadInt32();
        Field_0x7C = bs.ReadInt32();
        Field_0x80 = bs.ReadInt32();
        Field_0x84 = bs.ReadInt32();
        Field_0x88 = bs.ReadInt32();
        Field_0x8C = bs.ReadInt32();
        bs.ReadCheckPadding(0x18);
    }
}
