using System.Reflection;
using Syroot.BinaryData;

namespace FF16Tools.Files.CharaTimeline
{
    [AttributeUsage(AttributeTargets.Field)]
    public class OffsetAttribute : Attribute
    {
        public string TargetFieldName { get; }
        public Type TargetFieldType { get; }

        public string RelativeField { get; }

        public OffsetAttribute(string targetFieldName, Type targetFieldType, string relativeField = null)
        {
            TargetFieldName = targetFieldName;
            TargetFieldType = targetFieldType;
            RelativeField = relativeField;
        }

        public OffsetAttribute(string targetFieldName, string relativeField=null)
        {
            TargetFieldName = targetFieldName;
            TargetFieldType = typeof(string);
            RelativeField = relativeField;
        }
    }

    public abstract class BaseStruct
    {
        public abstract int _totalSize { get; }

        public Dictionary<string, string> _referencedStrings = new();
        public Dictionary<string, BaseStruct> _referencedStructs = new();
        public Dictionary<string, List<BaseStruct>> _referencedArrays = new();
        public byte[] _leftoverData;

        public FieldInfo[] GetAllFields()
        {
            return this.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .Where(f => !f.Name.StartsWith("_"))
                .ToArray();
        }

        public virtual void Read(BinaryStream bs) {
            long startingPos = bs.Position;
            Dictionary<string, long> fieldPos = new();
            fieldPos["UnionType"] = startingPos - 16;

            foreach (FieldInfo field in GetAllFields()) { 
                Type fieldType = field.FieldType;
                string fieldName = field.Name;

                fieldPos[fieldName] = bs.Position;

                switch (Type.GetTypeCode(fieldType))
                {
                    case TypeCode.Int32:
                        field.SetValue(this, bs.ReadInt32());
                        OffsetAttribute offsetAttr = field.GetCustomAttribute<OffsetAttribute>();

                        if (offsetAttr != null) {
                            long currentPos = bs.Position;
                            bs.Position = (int)field.GetValue(this) + (offsetAttr.RelativeField is null ? startingPos : fieldPos[offsetAttr.RelativeField]);
                            if (offsetAttr.TargetFieldType == typeof(string))
                                _referencedStrings[offsetAttr.TargetFieldName] = bs.ReadString(StringCoding.ZeroTerminated);
                            else {
                                var item = (BaseStruct)Activator.CreateInstance(offsetAttr.TargetFieldType);
                                item.Read(bs);
                                _referencedStructs[offsetAttr.TargetFieldName] = item;
                            }
                            bs.Position = currentPos;     
                        }
                        break;
                    case TypeCode.Single:
                        field.SetValue(this, bs.ReadSingle());
                        break;
                    case TypeCode.Double:
                        field.SetValue(this, bs.ReadDouble());
                        break;
                    case TypeCode.Byte:
                        field.SetValue(this, bs.Read1Byte());
                        break;
                    case TypeCode.Object:
                        if (fieldType.IsSubclassOf(typeof(BaseStruct))) {
                            var item = (BaseStruct)Activator.CreateInstance(fieldType);
                            item.Read(bs);
                            field.SetValue(this, item);
                        }
                        else if (fieldType.IsArray)
                        {
                            Type elementType = fieldType.GetElementType();
                            Array array = (Array)field.GetValue(this);
                            for (int i = 0; i < array.Length; i++)
                            {
                                if (elementType == typeof(int))
                                    array.SetValue(bs.ReadInt32(), i);
                                else if (elementType == typeof(float))
                                    array.SetValue(bs.ReadSingle(), i);
                                else if (elementType == typeof(double))
                                    array.SetValue(bs.ReadDouble(), i);
                                else if (elementType == typeof(byte))
                                    array.SetValue(bs.Read1Byte(), i);
                                else
                                    throw new NotImplementedException();
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
