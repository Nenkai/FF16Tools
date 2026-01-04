using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FF16Tools.Files.Magic.Factories;
using FF16Tools.Files.Magic.Operations;

namespace FF16Tools.Files.Magic;

public class OperationList : ISerializableStruct
{
    public List<IOperation> Operations { get; set; } = [];

    public void Read(SmartBinaryStream bs)
    {
        uint numProperties = bs.ReadUInt32();

        int i = 0;
        while (i < numProperties)
        {
            MagicOperationType type = (MagicOperationType)bs.ReadUInt32();
            bs.Position -= 4;

            IOperation operation = MagicOperationFactory.Create(type);
            operation.Read(bs);
            Operations.Add(operation);

            i += operation.Properties.Count;
        }
    }

    public void Write(SmartBinaryStream bs)
    {
        bs.WriteUInt32(GetNumberOfProperties());
        for (int i = 0; i < Operations.Count; i++)
        {
            Operations[i].Write(bs);
        }
    }

    public uint GetSize() => 0x0C;

    private uint GetNumberOfProperties()
    {
        uint count = 0;
        for (int i = 0; i < Operations.Count; i++)
            count += (uint)Operations[i].Properties.Count;
        return count;
    }
}
