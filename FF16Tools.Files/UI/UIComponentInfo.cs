using FF16Tools.Files.UI.Components;
using FF16Tools.Files.UI.Nodes;
using FF16Tools.Files.UI.Timelines;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI;

public class UIComponentInfo : ISerializableStruct
{
    public string Name { get; set; }
    public Size Size { get; set; }
    public UIComponentPropertiesBase Properties { get; set; }
    public List<GUINodeBase> Nodes { get; set; } = [];
    public List<UITimeline> Timelines { get; set; } = [];

    public uint GetSize()
    {
        return 0x40;
    }

    public void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        Name = bs.ReadStringPointer(basePos);
        Size = bs.ReadStructMarshal<Size>();
        uint componentPropertiesOffset = bs.ReadUInt32();
        uint nodesOffset = bs.ReadUInt32();
        uint nodeCount = bs.ReadUInt32();
        Timelines = bs.ReadStructArrayFromOffsetCount<UITimeline>(basePos);
        bs.ReadCheckPadding(0x20);

        bs.Position = basePos + componentPropertiesOffset;
        UIComponentType type = (UIComponentType)bs.ReadUInt32();
        Properties = UIComponentPropertiesBase.Create(type);
        bs.Position -= 4;
        Properties.Read(bs);

        for (int i = 0; i < nodeCount; i++)
        {
            bs.Position = basePos + nodesOffset + (i * sizeof(int));
            int nodeOffset = bs.ReadInt32();

            bs.Position = basePos + nodesOffset + nodeOffset;
            GUINodeType nodeType = (GUINodeType)bs.ReadInt32();
            GUINodeBase nodeBase = GUINodeBase.Create(nodeType);
            bs.Position -= 4;

            nodeBase.Read(bs);
            Nodes.Add(nodeBase);
        }
    }

    public void Write(SmartBinaryStream bs)
    {
        long basePos = bs.Position;
        long lastDataOffset = bs.GetMarker().LastDataPosition;

        bs.AddStringPointer(Name, basePos);
        bs.WriteStructMarshal(Size);
        bs.WriteStructPointer(basePos, Properties, ref lastDataOffset);
        bs.WriteStructArrayPointerWithOffsetTable32(basePos, Nodes, ref lastDataOffset);
        bs.WriteStructArrayPointer(basePos, Timelines, ref lastDataOffset);
        bs.WritePadding(0x20);

        bs.Position = lastDataOffset;
    }

    public override string ToString()
    {
        return $"{Name} [{Size}] ({Properties.Type}, {Timelines.Count} timelines)";
    }
}
