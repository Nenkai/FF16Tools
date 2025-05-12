using FF16Tools.Files.Timelines;
using Syroot.BinaryData;

using System.Reflection;

namespace FF16Tools.Files.Timelines.Chara;

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
    public override int TotalSize => -1;
    public abstract TimelineElementType ElementType { get; }
}

public class TimelineElementData : BaseStruct
{
    public override int TotalSize => -1;

    public int UnionType;
    public int field_0x04;
    public int field_0x08;
    public int field_0x0C;
    public TimelineElementDataInner ElementData;

    public override void Read(BinaryStream bs)
    {
        UnionType = bs.ReadInt32();
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
