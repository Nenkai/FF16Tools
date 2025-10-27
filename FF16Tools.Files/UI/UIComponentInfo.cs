using FF16Tools.Files.UI.Components;
using FF16Tools.Files.UI.Nodes;

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
    public Point Size { get; set; }
    public UIComponentPropertiesBase Properties { get; set; }

    public uint GetSize()
    {
        return 0x40;
    }

    public void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        Name = bs.ReadStringPointer(basePos);
        Size = bs.ReadPoint();
        uint componentPropertiesOffset = bs.ReadUInt32();
        uint nodesOffset = bs.ReadUInt32();
        uint nodeCount = bs.ReadUInt32();
        uint timelineElementsOffset = bs.ReadUInt32();
        uint timelineElementCount = bs.ReadUInt32();
        bs.ReadCheckPadding(0x20);

        bs.Position = basePos + componentPropertiesOffset;
        UIComponentType type = (UIComponentType)bs.ReadUInt32();
        Properties = type switch
        {
            UIComponentType.Root => new UIComponentRoot(),
            UIComponentType.Custom => new UIComponentCustom(),
            UIComponentType.Gauge => new UIComponentGauge(),
            _ => throw new NotImplementedException($"{type} not implemented")
        };
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
        }
    }

    public void Write(SmartBinaryStream bs)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return Name;
    }
}
