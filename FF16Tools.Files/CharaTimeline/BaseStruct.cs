using System.Reflection;
using Syroot.BinaryData;
using System.Collections;
using System.Runtime.InteropServices;


namespace FF16Tools.Files.CharaTimeline
{
    [AttributeUsage(AttributeTargets.Field)]
    public class RelativeField : Attribute
    {
        // The name of the field holding the offset to this field
        public string offsetFieldName;
        // The base field whose position will be added to the offset (usually the first field in the class)
        public string relativeToFieldName;

        public RelativeField(string offsetFieldName, string relativeToFieldName)
        {
            this.offsetFieldName = offsetFieldName;
            this.relativeToFieldName = relativeToFieldName;
        }

        public RelativeField(string offsetFieldName)
        {
            this.offsetFieldName = offsetFieldName;
            this.relativeToFieldName = null;
        }
    }

    public struct RelativeListInfo
    {
        public object OwningObject;
        public string fieldName;
        public List<BaseStruct> value;
    }

    public abstract class BaseStruct
    {
        public abstract int _totalSize { get; }
        public byte[] _leftoverData;

        public FieldInfo[] GetAllFields()
        {
            return this.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .Where(f => !f.Name.StartsWith("_"))
                .ToArray();
        }

        private static bool IsListOfStructs(FieldInfo field)
        {
            return field.FieldType.IsGenericType &&
                   field.FieldType.GetGenericTypeDefinition() == typeof(List<>) &&
                   field.FieldType.GetGenericArguments().First().IsSubclassOf(typeof(BaseStruct));
        }

        public List<string> GetAllStrings()
        {
            var x = GetAllFields()
                .Where(f => f.FieldType == typeof(string))
                .Select(f => (string)f.GetValue(this)).Concat(
                    GetAllFields()
                    .Where(f => f.FieldType.IsSubclassOf(typeof(BaseStruct)))
                    .Select(f => ((BaseStruct)f.GetValue(this))).Where(f => f != null)
                    .SelectMany(f => f.GetAllStrings())
                ).ToList();
            return x;
        }
        public List<RelativeListInfo> GetAllRelativeLists()
        {
            // Get all fields that are a list of subclasses of BaseStruct and are a relativeField
            var x = GetAllFields()
                .Where(f => IsListOfStructs(f) && f.GetCustomAttribute<RelativeField>() != null)
                .Select(f => new RelativeListInfo
                {
                    OwningObject = this,
                    fieldName = f.Name,
                    value = ((IEnumerable)f.GetValue(this))
                             .Cast<BaseStruct>()
                             .ToList()
                })
                .ToList();
            return x;
        }

        private int GetFieldSize(FieldInfo field)
        {
            if (field.FieldType.IsSubclassOf(typeof(BaseStruct)))
            {
                return ((BaseStruct)field.GetValue(this)).GetNonRelativeSize();
            }
            if (field.FieldType.IsArray)
            {
                Type elemType = field.FieldType.GetElementType();
                return Marshal.SizeOf(elemType) * ((Array)field.GetValue(this)).Length;
            }
            return Marshal.SizeOf(field.FieldType);
        }

        public int GetNonRelativeSize()
        {
            if (_totalSize != -1)
                return _totalSize;

            return GetAllFields().
                Where(f => f.GetCustomAttribute<RelativeField>() == null).
                Select(GetFieldSize).Sum() + (_leftoverData != null ? _leftoverData.Length : 0);
        }

        public virtual void Read(BinaryStream bs)
        {
            long startingPos = bs.Position;
            Dictionary<string, long> fieldPos = new();
            fieldPos["UnionType"] = startingPos - 16; // For relative fields that are relative to the parent data union

            foreach (FieldInfo field in GetAllFields())
            {
                Type fieldType = field.FieldType;
                string fieldName = field.Name;

                fieldPos[fieldName] = bs.Position;

                switch (Type.GetTypeCode(fieldType))
                {
                    case TypeCode.Int32:
                        field.SetValue(this, bs.ReadInt32());
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

                    case TypeCode.String:
                        RelativeField relativeAttr = field.GetCustomAttribute<RelativeField>();
                        long currentPos = bs.Position;
                        bs.Position = (int)this.GetType().GetField(relativeAttr.offsetFieldName).GetValue(this) +
                            (relativeAttr.relativeToFieldName == null ? startingPos : fieldPos[relativeAttr.relativeToFieldName]);

                        field.SetValue(this, bs.ReadString(StringCoding.ZeroTerminated));
                        bs.Position = currentPos;
                        break;
                    case TypeCode.Object:
                        long currentPos2 = bs.Position;
                        RelativeField relativeAttr2 = field.GetCustomAttribute<RelativeField>();
                        if (relativeAttr2 != null)
                        {
                            bs.Position = (int)this.GetType().GetField(relativeAttr2.offsetFieldName).GetValue(this) +
                                (relativeAttr2.relativeToFieldName == null ? startingPos : fieldPos[relativeAttr2.relativeToFieldName]);
                        }

                        if (fieldType.IsSubclassOf(typeof(BaseStruct)))
                        {
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

                        if (relativeAttr2 != null)
                            bs.Position = currentPos2;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            if (_totalSize != -1 && bs.Position - startingPos < _totalSize)
                _leftoverData = bs.ReadBytes((int)(_totalSize - (bs.Position - startingPos))).ToArray();
        }

        public virtual void Write(BinaryStream bs, Dictionary<(object, string), long> relativeFieldPos, Dictionary<string, long> stringPos)
        {
            long startingPos = bs.Position;
            Dictionary<string, long> fieldPos = new();
            fieldPos["UnionType"] = startingPos - 16;

            foreach (FieldInfo field in GetAllFields().Where(f => f.GetCustomAttribute<RelativeField>() == null)) {
                fieldPos[field.Name] = bs.Position;
                switch (Type.GetTypeCode(field.FieldType))
                {
                    case TypeCode.Int32:
                        bs.WriteInt32((int)field.GetValue(this));
                        break;
                    case TypeCode.Single:
                        bs.WriteSingle((float)field.GetValue(this));
                        break;
                    case TypeCode.Double:
                        bs.WriteDouble((double)field.GetValue(this));
                        break;
                    case TypeCode.Byte:
                        bs.WriteByte((byte)field.GetValue(this));
                        break;
                    case TypeCode.Object:
                        if (field.FieldType.IsSubclassOf(typeof(BaseStruct)))
                        {
                            ((BaseStruct)field.GetValue(this)).Write(bs, relativeFieldPos, stringPos);
                        }
                        else if (field.FieldType.IsArray)
                        {
                            Type elementType = field.FieldType.GetElementType();
                            Array array = (Array)field.GetValue(this);
                            for (int i = 0; i < array.Length; i++)
                            {
                                if (elementType == typeof(int))
                                    bs.WriteInt32((int)array.GetValue(i));
                                else if (elementType == typeof(float))
                                    bs.WriteSingle((float)array.GetValue(i));
                                else if (elementType == typeof(double))
                                    bs.WriteDouble((double)array.GetValue(i));
                                else if (elementType == typeof(byte))
                                    bs.WriteByte((byte)array.GetValue(i));
                                else
                                    throw new NotImplementedException();
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            if (_leftoverData != null)
            {
                bs.WriteBytes(_leftoverData);
            }

            long endPos = bs.Position;

            foreach (FieldInfo field in GetAllFields().Where(f=> f.GetCustomAttribute<RelativeField>() != null))
            {
                RelativeField relativeAttr = field.GetCustomAttribute<RelativeField>();
                // The position of the field that will hold the offset value
                long offsetFieldPos = fieldPos[relativeAttr.offsetFieldName];
                // The position of the field that the offset is relative to
                long offsetRelativeToPos = relativeAttr.relativeToFieldName != null ? fieldPos[relativeAttr.relativeToFieldName] : startingPos;
                // Where the relative value is actually written
                long relativeValuePos = field.FieldType == typeof(string) ? stringPos[(string)field.GetValue(this)] : relativeFieldPos[(this, field.Name)];
                // The offset value to write
                int offsetToValue = (int)(relativeValuePos - offsetRelativeToPos);

                bs.Position = offsetFieldPos;
                bs.WriteInt32(offsetToValue);
            }

            bs.Position = endPos;
        }
    }
}
