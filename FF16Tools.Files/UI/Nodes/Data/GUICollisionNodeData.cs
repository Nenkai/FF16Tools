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
    public int Field_0x40 { get; set; }

    public override uint GetSize()
    {
        return base.GetSize() + 0x20;
    }

    public override void Read(SmartBinaryStream bs)
    {
        base.Read(bs);
        Field_0x40 = bs.ReadInt32();
        bs.ReadCheckPadding(0x1C);
    }

    public override void WriteExtraData(SmartBinaryStream bs, long basePos, ref long lastDataOffset)
    {
        bs.WriteInt32(Field_0x40);
        bs.WritePadding(0x1C);
    }
}
