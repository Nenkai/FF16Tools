using FF16Tools.Files.UI.Assets;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Timelines.Targets;

public abstract class UITimelineTargetBase : ISerializableStruct
{
    /// <summary>
    /// 4001, or 4002
    /// </summary>
    public uint TargetTypeId { get; set; }

    public virtual uint GetSize()
    {
        return 0x28;
    }

    public virtual void Read(SmartBinaryStream bs)
    {
        TargetTypeId = bs.ReadUInt32();
        bs.ReadCheckPadding(0x24);
    }

    public abstract void Write(SmartBinaryStream bs);
}
