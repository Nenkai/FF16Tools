using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

    public static MagicPropertyValueBase? GetValue(MagicPropertyType type, byte[] bytes)
    {
        if (!TypeToValueType.TryGetValue(type, out MagicPropertyValueType valueType))
            return null;

        return valueType switch
        {
            MagicPropertyValueType.OperationGroupIdValue => new MagicPropertyIdValue(BinaryPrimitives.ReadInt32LittleEndian(bytes)),
            MagicPropertyValueType.IntValue => new MagicPropertyIntValue(BinaryPrimitives.ReadInt32LittleEndian(bytes)),
            MagicPropertyValueType.FloatValue => new MagicPropertyFloatValue(BinaryPrimitives.ReadSingleLittleEndian(bytes)),
            MagicPropertyValueType.ByteValue => new MagicPropertyByteValue(bytes[0]),
            MagicPropertyValueType.BoolValue => new MagicPropertyBoolValue(bytes[0] != 0),
            _ => null,
        };
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
}

public abstract class MagicPropertyValueBase
{
    public abstract byte[] GetBytes();
}

public class MagicPropertyByteValue(byte value) : MagicPropertyValueBase
{
    public byte Value = value;

    public override byte[] GetBytes()
    {
        byte[] bytes = new byte[4];
        bytes[0] = Value;
        return bytes;
    }
}

public class MagicPropertyIdValue(int id) : MagicPropertyValueBase
{
    public int Id = id;

    public override byte[] GetBytes()
    {
        byte[] bytes = new byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(bytes, Id);
        return bytes;
    }
}

public class MagicPropertyIntValue(int value) : MagicPropertyValueBase
{
    public int Value = value;

    public override byte[] GetBytes()
    {
        byte[] bytes = new byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(bytes, Value);
        return bytes;
    }
}

public class MagicPropertyFloatValue(float value) : MagicPropertyValueBase
{
    public float Value = value;

    public override byte[] GetBytes()
    {
        byte[] bytes = new byte[4];
        BinaryPrimitives.WriteSingleLittleEndian(bytes, Value);
        return bytes;
    }
}

public class MagicPropertyBoolValue(bool value) : MagicPropertyValueBase
{
    public bool Value = value;

    public override byte[] GetBytes()
    {
        byte[] bytes = new byte[4];
        bytes[0] = Value ? (byte)1 : (byte)0;
        return bytes;
    }
}
