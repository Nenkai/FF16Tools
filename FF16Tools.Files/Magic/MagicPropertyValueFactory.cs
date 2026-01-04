using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace FF16Tools.Files.Magic;

public class MagicPropertyValueFactory
{
    public static Dictionary<MagicPropertyType, MagicPropertyValueType> TypeToValueType { get; private set; } = new()
    {
        [MagicPropertyType.Prop_2] = MagicPropertyValueType.IntValue,
        [MagicPropertyType.Prop_3] = MagicPropertyValueType.FloatValue,
        [MagicPropertyType.Prop8_SpeedStart] = MagicPropertyValueType.FloatValue,
        [MagicPropertyType.Prop13_NoTrackingTarget] = MagicPropertyValueType.BoolValue,
        [MagicPropertyType.Prop14_UnkMaxAngleRad] = MagicPropertyValueType.FloatValue,
        [MagicPropertyType.Prop22_VerticalAngleDegreesOffset] = MagicPropertyValueType.FloatValue,
        [MagicPropertyType.Prop_26] = MagicPropertyValueType.IntValue,
        [MagicPropertyType.Prop_VFXAudioId] = MagicPropertyValueType.IntValue,
        [MagicPropertyType.Prop_30] = MagicPropertyValueType.IntValue,
        [MagicPropertyType.Prop_VFXScale] = MagicPropertyValueType.FloatValue,
        [MagicPropertyType.Prop_ProjectileDuration] = MagicPropertyValueType.FloatValue,
        [MagicPropertyType.Prop_ProjectileDurationRandomRange] = MagicPropertyValueType.Vec3Value,
        [MagicPropertyType.Prop_OnNoImpactOperationGroupIdCallback] = MagicPropertyValueType.OperationGroupIdValue,
        [MagicPropertyType.Prop_42] = MagicPropertyValueType.FloatValue,
        [MagicPropertyType.Prop_45] = MagicPropertyValueType.IntValue,
        [MagicPropertyType.Prop_46] = MagicPropertyValueType.FloatValue,
        [MagicPropertyType.Prop_AttackParamId] = MagicPropertyValueType.IntValue,
        [MagicPropertyType.Prop_73] = MagicPropertyValueType.IntValue,
        [MagicPropertyType.Prop_81] = MagicPropertyValueType.IntValue,
        [MagicPropertyType.Prop_89] = MagicPropertyValueType.IntValue,
        [MagicPropertyType.Prop95_OnTargetHitOperationGroupIdCallback] = MagicPropertyValueType.OperationGroupIdValue,
        [MagicPropertyType.Prop_2227] = MagicPropertyValueType.IntValue,
        [MagicPropertyType.Prop_2575] = MagicPropertyValueType.IntValue,
    };

    public static MagicPropertyValueBase? GetValue(MagicPropertyType propertyType, byte[] bytes)
    {
        if (!TypeToValueType.TryGetValue(propertyType, out MagicPropertyValueType valueType))
            return null;

        return valueType switch
        {
            MagicPropertyValueType.OperationGroupIdValue => new MagicPropertyIdValue(BinaryPrimitives.ReadInt32LittleEndian(bytes)),
            MagicPropertyValueType.IntValue => new MagicPropertyIntValue(BinaryPrimitives.ReadInt32LittleEndian(bytes)),
            MagicPropertyValueType.FloatValue => new MagicPropertyFloatValue(BinaryPrimitives.ReadSingleLittleEndian(bytes)),
            MagicPropertyValueType.ByteValue => new MagicPropertyByteValue(bytes[0]),
            MagicPropertyValueType.BoolValue => new MagicPropertyBoolValue(bytes[0] != 0),
            MagicPropertyValueType.Vec3Value => new MagicPropertyVec3Value(MemoryMarshal.Cast<byte, Vector3>(bytes)[0]),
            _ => null,
        };
    }

    public static MagicOperationProperty? CreateDefault(MagicPropertyType propertyType)
    {
        if (!TypeToValueType.TryGetValue(propertyType, out MagicPropertyValueType valueType))
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

    /// <summary>
    /// Used for mapping additional properties not supported by default.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="valueType"></param>
    public static void Add(MagicPropertyType type, MagicPropertyValueType valueType)
    {
        TypeToValueType.TryAdd(type, valueType);
    }
}

public enum MagicPropertyValueType
{
    OperationGroupIdValue,
    IntValue,
    FloatValue,
    ByteValue,
    BoolValue,
    Vec3Value,
}

public abstract class MagicPropertyValueBase
{
    public abstract byte[] GetBytes();
}

public class MagicPropertyByteValue : MagicPropertyValueBase
{
    public byte Value;

    public MagicPropertyByteValue() 
        => Value = 0; 

    public MagicPropertyByteValue(byte value)
        => Value = value;

    public override byte[] GetBytes()
    {
        byte[] bytes = new byte[4];
        bytes[0] = Value;
        return bytes;
    }
}

public class MagicPropertyIdValue : MagicPropertyValueBase
{
    public int Id;

    public MagicPropertyIdValue()
        => Id = 0;

    public MagicPropertyIdValue(int id)
        => Id = id;

    public override byte[] GetBytes()
    {
        byte[] bytes = new byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(bytes, Id);
        return bytes;
    }
}

public class MagicPropertyIntValue : MagicPropertyValueBase
{
    public int Value;

    public MagicPropertyIntValue()
        => Value = 0;

    public MagicPropertyIntValue(int value)
        => Value = value;

    public override byte[] GetBytes()
    {
        byte[] bytes = new byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(bytes, Value);
        return bytes;
    }
}

public class MagicPropertyFloatValue : MagicPropertyValueBase
{
    public float Value;

    public MagicPropertyFloatValue()
        => Value = 0;

    public MagicPropertyFloatValue(float value)
        => Value = value;

    public override byte[] GetBytes()
    {
        byte[] bytes = new byte[4];
        BinaryPrimitives.WriteSingleLittleEndian(bytes, Value);
        return bytes;
    }
}

public class MagicPropertyBoolValue : MagicPropertyValueBase
{
    public bool Value;

    public MagicPropertyBoolValue()
        => Value = false;

    public MagicPropertyBoolValue(bool value)
        => Value = value;

    public override byte[] GetBytes()
    {
        byte[] bytes = new byte[4];
        bytes[0] = Value ? (byte)1 : (byte)0;
        return bytes;
    }
}

public class MagicPropertyVec3Value : MagicPropertyValueBase
{
    public Vector3 Value;

    public MagicPropertyVec3Value()
        => Value = Vector3.Zero;

    public MagicPropertyVec3Value(Vector3 value)
        => Value = value;

    public override byte[] GetBytes()
    {
        byte[] bytes = new byte[0x0C];
        BinaryPrimitives.WriteSingleLittleEndian(bytes.AsSpan(0x00), Value.X);
        BinaryPrimitives.WriteSingleLittleEndian(bytes.AsSpan(0x04), Value.Y);
        BinaryPrimitives.WriteSingleLittleEndian(bytes.AsSpan(0x08), Value.Z);
        return bytes;
    }
}