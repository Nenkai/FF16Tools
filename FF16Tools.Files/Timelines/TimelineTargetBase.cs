namespace FF16Tools.Files.Timelines;

public class TimelineTargetBase : ISerializableStruct
{
    public uint Type { get; set; }

    public virtual void Read(SmartBinaryStream bs)
    {
        Type = bs.ReadUInt32();
        bs.ReadCheckPadding(0x24);
    }

    public virtual void Write(SmartBinaryStream bs)
    {
        bs.WriteUInt32(Type);
        bs.WritePadding(0x24);
    }

    public virtual uint GetSize() => 0x28;
}
