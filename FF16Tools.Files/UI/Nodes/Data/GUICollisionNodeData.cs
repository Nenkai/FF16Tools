using FF16Tools.Files.UI.Assets;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes.Data;

public class GUICollisionNodeData : GUINodeDataBase
{
    public override void Read(SmartBinaryStream bs)
    {
        base.Read(bs);
        bs.ReadCheckPadding(0x20);
    }
}
