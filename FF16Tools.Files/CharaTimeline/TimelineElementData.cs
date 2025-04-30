using Syroot.BinaryData;

using System.Reflection;

namespace FF16Tools.Files.CharaTimeline;

public static class DataElementsRegistry
{
    public static readonly Dictionary<int, Type> TypeIdToElementType = BuildElementDictionary();

    public static Dictionary<int, Type> BuildElementDictionary()
    {
        Dictionary<int, Type> dict = new();
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.IsSubclassOf(typeof(TimelineElementDataInner)))
                dict[int.Parse(type.Name.Split('_').Last())] = type;
        }

        return dict;
    }
}

public abstract class TimelineElementDataInner : BaseStruct
{
    public override int _totalSize => -1;
    public string _elementTypeName = null;
}

public class TimelineElementData : BaseStruct
{
    public override int _totalSize => 0x20;

    public int UnionType;
    int field_0x04;
    int field_0x08;
    int field_0x0C;
    public TimelineElementDataInner ElementData;

    public override void Read(BinaryStream bs)
    {
        UnionType = bs.ReadInt32(); // go -16 to start from UnionType
        field_0x04 = bs.ReadInt32();
        field_0x08 = bs.ReadInt32();
        field_0x0C = bs.ReadInt32();
        if (DataElementsRegistry.TypeIdToElementType.ContainsKey(UnionType))
        {
            Type elementType = DataElementsRegistry.TypeIdToElementType[UnionType];
            ElementData = (TimelineElementDataInner)Activator.CreateInstance(elementType);
            ElementData.Read(bs);
        }
        else {
            ElementData = null;
        }
        
    }
}
