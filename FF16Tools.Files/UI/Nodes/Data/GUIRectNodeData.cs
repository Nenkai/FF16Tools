using FF16Tools.Files.UI.Assets;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes.Data;

public class GUIRectNodeData : GUINodeDataBase
{
    public int Field_0x40 { get; set; }
    public int Field_0x44 { get; set; }
    public int Field_0x48 { get; set; }
    public int Field_0x4C { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        base.Read(bs);
        Field_0x40 = bs.ReadInt32();
        Field_0x44 = bs.ReadInt32();
        Field_0x48 = bs.ReadInt32();
        Field_0x4C = bs.ReadInt32();
        bs.ReadCheckPadding(0x10);
    }
}
