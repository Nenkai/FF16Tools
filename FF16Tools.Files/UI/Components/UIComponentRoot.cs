using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Components;

public class UIComponentRoot : UIComponentPropertiesBase
{
    public uint Field_0x108 { get; set; }
    public uint Field_0x10C { get; set; }

    public override uint GetSize()
    {
        return base.GetSize() + 0x08;
    }

    public override void Read(SmartBinaryStream bs)
    {
        base.Read(bs);
        Field_0x108 = bs.ReadUInt32();
        Field_0x10C = bs.ReadUInt32();
        bs.ReadCheckPadding(0x18);
    }
}
