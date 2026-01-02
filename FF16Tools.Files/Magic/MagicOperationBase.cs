using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic;

public abstract class MagicOperationBase<T> : IOperation
    where T : IOperationBase<T>
{
    public abstract MagicOperationType Type { get; }
    public HashSet<MagicPropertyType> SupportedProperties => T.sSupportedProperties;

    public List<MagicOperationProperty> Properties { get; set; } = [];

    public uint GetSize()
    {
        throw new NotImplementedException();
    }

    public void Read(SmartBinaryStream bs)
    {
        MagicOperationType type = (MagicOperationType)bs.ReadUInt32();
        Debug.Assert(type == Type);

        uint numProperties = bs.ReadUInt32();
        for (int i = 0; i < numProperties; i++)
        {
            var prop = new MagicOperationProperty();
            prop.Read(bs);

            if (!T.sSupportedProperties.Contains(prop.Type))
                throw new NotSupportedException($"Property type {prop.Type} is not supported for this operation ({Type})");

            Properties.Add(prop);
        }
    }

    public void Write(SmartBinaryStream bs)
    {
        bs.WriteUInt32((uint)Type);
        bs.WriteUInt32((uint)Properties.Count);
        foreach (var prop in Properties)
            prop.Write(bs);
    }
}

public interface IOperationBase<T> : IOperation
    where T : IOperationBase<T>
{
    static abstract HashSet<MagicPropertyType> sSupportedProperties { get; set; }
}

public interface IOperation : ISerializableStruct
{
    MagicOperationType Type { get; }
    List<MagicOperationProperty> Properties { get; }
    HashSet<MagicPropertyType> SupportedProperties { get; }
}