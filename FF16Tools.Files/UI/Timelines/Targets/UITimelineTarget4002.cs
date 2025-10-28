using FF16Tools.Files.UI.Assets;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Timelines.Targets;

public class UITimelineTarget4002 : UITimelineTargetBase
{
    public override uint GetSize()
    {
        return base.GetSize();
    }

    public override void Read(SmartBinaryStream bs)
    {
        base.Read(bs);
    }

    public override void Write(SmartBinaryStream bs)
    {
        throw new NotImplementedException();
    }
}
