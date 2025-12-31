using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic;

public class MagicOperation
{
    public uint Type { get; set; }
    public List<MagicOperationProperty> Properties { get; set; } = [];

    public void Read(SmartBinaryStream bs)
    {
        Type = bs.ReadUInt32();
        uint numProperties = bs.ReadUInt32();
        for (int i = 0; i < numProperties; i++)
        {
            var prop = new MagicOperationProperty();
            prop.Read(bs);
            Properties.Add(prop);
        }
    }

    public void Write(SmartBinaryStream bs)
    {
        bs.WriteUInt32(Type);
        bs.WriteUInt32((uint)Properties.Count);
        foreach (var prop in Properties)
            prop.Write(bs);
    }
}
