using FF16Tools.Files.UI.Assets;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes.Data;

public class GUIEffectNodeData : GUINodeDataBase
{
    public override uint GetSize()
    {
        return base.GetSize() + 0x20;
    }

    public override void Read(SmartBinaryStream bs)
    {
        base.Read(bs);
        bs.ReadCheckPadding(0x20);
    }

    public override void WriteExtraData(SmartBinaryStream bs, long basePos, ref long lastDataOffset)
    {
        bs.WritePadding(0x20);
    }
}
