using FF16Tools.Files.Timelines;
using FF16Tools.Files.Timelines.Chara;
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
    public Timeline Timeline { get; set; } = new();
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
        Timeline.Read(bs);
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
        long basePos = bs.Position;
        long lastDataPos = bs.GetMarker().LastDataPosition;

        bs.AddStringPointer(Name, basePos);
        bs.WriteUInt32(Flags);
        using (var marker = bs.PushMarker(lastDataPos))
        {
            Timeline.Write(bs);
            lastDataPos = marker.LastDataPosition;
        }

        bs.WriteUInt32(Field_0x2C);
        bs.AddStringPointer(Str_0x30, basePos);
        bs.WriteStructPointer(basePos, UnkAssetReference_0x34, ref lastDataPos);
        bs.WriteUInt32(Field_0x38);
        bs.AddStringPointer(Str_0x3C, basePos);
        bs.WriteUInt32(Field_0x40);
        bs.WritePadding(0x1C);

        bs.Position = lastDataPos;
    }
}
