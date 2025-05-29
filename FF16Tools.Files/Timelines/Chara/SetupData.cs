namespace FF16Tools.Files.Timelines.Chara;

public class SetupData : ISerializableStruct
{
    public uint Type { get; set; }

    public void Read(SmartBinaryStream bs)
    {
        Type = bs.ReadUInt32();
        bs.ReadCheckPadding(0x24);
    }

    public void Write(SmartBinaryStream bs)
    {
        bs.WriteUInt32(Type);
        bs.WritePadding(0x24);
    }

    public uint GetSize() => 0x28;
}
