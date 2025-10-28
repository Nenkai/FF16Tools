using FF16Tools.Files.UI.Assets;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Timelines.Targets;

public class UITimelineComponentTarget : UITimelineTargetBase
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
        throw new NotImplementedException();
    }
}
