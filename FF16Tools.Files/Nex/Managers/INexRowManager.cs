using FF16Tools.Files.Nex.Entities;
using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Nex.Managers;

public interface INexRowManager
{
    public void Read(BinaryStream bs);

    public NexRowInfo GetRowInfo(uint rowId, uint subId = 0, uint arrayIndex = 0);

    public List<NexRowInfo> GetAllRowInfos();
}
