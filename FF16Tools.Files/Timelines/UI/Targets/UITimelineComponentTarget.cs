using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.UI.Targets;

public class UITimelineComponentTarget : TimelineTargetBase
{
    public string ComponentName { get; set; }

    public override uint GetSize()
    {
        return 0x2C;
    }

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;
        base.Read(bs);
        ComponentName = bs.ReadStringPointer(basePos);
    }

    public override void Write(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        base.Write(bs);
        bs.AddStringPointer(ComponentName, basePos);
    }
}
