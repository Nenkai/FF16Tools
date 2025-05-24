namespace FF16Tools.Files.Timelines.Chara;

public class InternalFinalStruct : ISerializableStruct
{
    public int UnkType;

    public void Read(SmartBinaryStream bs)
    {
        UnkType = bs.ReadInt32();
        bs.ReadCheckPadding(0x24);
    }

    public void Write(SmartBinaryStream bs)
    {
        bs.WriteInt32(UnkType);
        bs.WritePadding(0x24);
    }


    public uint GetSize() => 0x28;
}

public class FinalStruct : ISerializableStruct
{
    public InternalFinalStruct InternalFinalStruct = new();

    public void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        int dataOffset = bs.ReadInt32();

        bs.Position = basePos + dataOffset;
        InternalFinalStruct.Read(bs);

        bs.Position = basePos + GetSize();
    }

    public void Write(SmartBinaryStream bs)
    {
        long basePos = bs.Position;
        bs.WriteUInt32(0x04); // offset

        bs.Position = basePos + GetSize();
        InternalFinalStruct.Write(bs);

        bs.Position = basePos + GetSize();
    }

    public uint GetSize() => 0x04;
}
