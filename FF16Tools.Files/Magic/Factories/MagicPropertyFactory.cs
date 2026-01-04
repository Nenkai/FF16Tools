using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Factories;

public class MagicPropertyFactory
{
    public static MagicOperationProperty? Create(MagicPropertyType propertyType)
    {
        if (!MagicPropertyValueTypeMapping.TypeToValueType.TryGetValue(propertyType, out MagicPropertyValueType valueType))
            return null;

        var prop = new MagicOperationProperty(propertyType);
        prop.Value = valueType switch
        {
            MagicPropertyValueType.OperationGroupIdValue => new MagicPropertyIdValue(),
            MagicPropertyValueType.IntValue => new MagicPropertyIntValue(),
            MagicPropertyValueType.FloatValue => new MagicPropertyFloatValue(),
            MagicPropertyValueType.ByteValue => new MagicPropertyByteValue(),
            MagicPropertyValueType.BoolValue => new MagicPropertyBoolValue(),
            MagicPropertyValueType.Vec3Value => new MagicPropertyVec3Value(),
            _ => throw new NotSupportedException($"Property value type '{valueType}' not supported"),
        };
        prop.Data = prop.Value.GetBytes();
        return prop;
    }
}
