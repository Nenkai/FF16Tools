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
    public uint Field_0x40 { get; set; }
    public uint Field_0x44 { get; set; }
    public uint Field_0x48 { get; set; }
    public uint Field_0x4C { get; set; }

    public override uint GetSize()
    {
        return base.GetSize() + 0x20;
    }

    public override void Read(SmartBinaryStream bs)
    {
        base.Read(bs);
        Field_0x40 = bs.ReadUInt32();
        Field_0x44 = bs.ReadUInt32();
        Field_0x48 = bs.ReadUInt32();
        Field_0x4C = bs.ReadUInt32();
        bs.ReadCheckPadding(0x10);
    }

    public override void WriteExtraData(SmartBinaryStream bs, long basePos, ref long lastDataOffset)
    {
        bs.WriteUInt32(Field_0x40);
        bs.WriteUInt32(Field_0x44);
        bs.WriteUInt32(Field_0x48);
        bs.WriteUInt32(Field_0x4C);
        bs.WritePadding(0x10);
    }
}
