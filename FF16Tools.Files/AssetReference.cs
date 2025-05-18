using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files;

public class AssetReference : ISerializableStruct
{
    public AssetReferenceType AssetType { get; set; }
    public string? Path { get; set; }

    public void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        AssetType = (AssetReferenceType)bs.ReadUInt32();
        Path = bs.ReadStringPointer(basePos);
    }

    public void Write(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        bs.WriteUInt32((uint)AssetType);
        bs.AddStringPointer(Path, basePos);
    }

    public uint GetSize()
    {
        return 0x08;
    }
}

public enum AssetReferenceType : uint
{
    None = 0,

    /// <summary>
    /// .anmb file
    /// </summary>
    Animation = 1009,

    /// <summary>
    /// .sab file
    /// </summary>
    Audio = 1012,

    /// <summary>
    /// .vfxb file
    /// </summary>
    VFX = 1019,
}
