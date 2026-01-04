using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace FF16Tools.Files.Magic.Factories;

public class MagicPropertyValueFactory
{
    public static MagicPropertyValueBase? GetValue(MagicPropertyType propertyType, byte[] bytes)
    {
        if (!MagicPropertyValueTypeMapping.TypeToValueType.TryGetValue(propertyType, out MagicPropertyValueType valueType))
            return null;

        return valueType switch
        {
            MagicPropertyValueType.OperationGroupId => new MagicPropertyIdValue(BinaryPrimitives.ReadInt32LittleEndian(bytes)),
            MagicPropertyValueType.Int => new MagicPropertyIntValue(BinaryPrimitives.ReadInt32LittleEndian(bytes)),
            MagicPropertyValueType.Float => new MagicPropertyFloatValue(BinaryPrimitives.ReadSingleLittleEndian(bytes)),
            MagicPropertyValueType.Byte => new MagicPropertyByteValue(bytes[0]),
            MagicPropertyValueType.Bool => new MagicPropertyBoolValue(bytes[0] != 0),

            // Some vec3 properties may only store one axis
            MagicPropertyValueType.Vec3 => new MagicPropertyVec3Value(
                bytes.Length == 0x0C ? MemoryMarshal.Cast<byte, Vector3>(bytes)[0] : new Vector3(BinaryPrimitives.ReadSingleLittleEndian(bytes), 0, 0)),
            _ => null,
        };
    }
}

public enum MagicPropertyValueType
{
    OperationGroupId,
    Int,
    Float,
    Byte,
    Bool,
    Vec3,
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