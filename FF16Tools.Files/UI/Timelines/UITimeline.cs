using FF16Tools.Files.UI.Assets;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Timelines;

public class UITimeline : ISerializableStruct
{
    public string Name { get; set; }
    public uint Flags { get; set; }
    public UITimelineInfo TimelineInfo { get; set; } = new();
    public uint Field_0x2C { get; set; }
    public string Str_0x30 { get; set; }
    public UIAssetReference UnkAssetReference_0x34 { get; set; }
    public uint Field_0x38 { get; set; }
    public string Str_0x3C { get; set; }
    public uint Field_0x40 { get; set; }

    public uint GetSize()
    {
        return 0x60;
    }

    public void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;
        Name = bs.ReadStringPointer(basePos);
        Flags = bs.ReadUInt32();
        TimelineInfo.Read(bs);
        Field_0x2C = bs.ReadUInt32();
        Str_0x30 = bs.ReadStringPointer(basePos);
        UnkAssetReference_0x34 = bs.ReadStructPointer<UIAssetReference>(basePos);
        Field_0x38 = bs.ReadUInt32();
        Str_0x3C = bs.ReadStringPointer(basePos);
        Field_0x40 = bs.ReadUInt32();
        bs.ReadCheckPadding(0x1C);
    }

    public void Write(SmartBinaryStream bs)
    {
        throw new NotImplementedException();
    }
}
