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
            MagicPropertyValueType.OperationGroupId => new MagicPropertyIdValue(),
            MagicPropertyValueType.Int => new MagicPropertyIntValue(),
            MagicPropertyValueType.Float => new MagicPropertyFloatValue(),
            MagicPropertyValueType.Byte => new MagicPropertyByteValue(),
            MagicPropertyValueType.Bool => new MagicPropertyBoolValue(),
            MagicPropertyValueType.Vec3 => new MagicPropertyVec3Value(),
            _ => throw new NotSupportedException($"Property value type '{valueType}' not supported"),
        };
        prop.Data = prop.Value.GetBytes();
        return prop;
    }
}
