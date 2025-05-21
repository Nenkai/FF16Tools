using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Model.ExternalContent;

public class MdlExternalContentHeader
{
    public static uint MAGIC => BinaryPrimitives.ReadUInt32LittleEndian("MCEX"u8);

    // It seems only type 1 and 9 are ever used.
    public Dictionary<MdlExternalContentType, MdlExternalContentTypeBase> Entries { get; set; } = [];

    public void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        uint magic = bs.ReadUInt32();
        if (magic != MAGIC)
            throw new InvalidDataException($"Model file has incorrect external content magic (expected {MAGIC:X4}, got {magic:X4}).");

        uint version = bs.ReadUInt32();
        Debug.Assert(version == 1, $"Expected MCEX version 1, got {version}");

        bs.Position += 0x18;

        uint entryOffsetsOffset = bs.ReadUInt32();
        uint entryCount = bs.ReadUInt32();

        for (int i = 0; i < entryCount; i++)
        {
            bs.Position = basePos + entryOffsetsOffset + i * 0x04;
            uint entryOffset = bs.ReadUInt32();

            bs.Position = basePos + entryOffsetsOffset + entryOffset;
            MdlExternalContentType type = (MdlExternalContentType)bs.ReadUInt32();
            bs.Position -= 4;

            MdlExternalContentTypeBase base_;
            switch (type)
            {
                case MdlExternalContentType.Unk1:
                    base_ = new MdlExternalContentType1();
                    break;

                case MdlExternalContentType.Unk2:
                    throw new NotImplementedException("MCEX type 2 is not yet implemented");

                case MdlExternalContentType.EidMap:
                    base_ = new MdlExternalContentEidMap();
                    break;

                default:
                    throw new NotSupportedException();

            }
            base_.Read(bs);
            Entries.Add(type, base_);
        }
    }
}

public enum MdlExternalContentType : uint
{
    Unk1 = 1,
    Unk2 = 2, // Not used by any models (didn't check DLC). .pcb file related?
    EidMap = 9,
}

public abstract class MdlExternalContentTypeBase 
{
    public MdlExternalContentType Type { get; set; }

    public abstract void Read(SmartBinaryStream bs);
}

