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

    public NexRowInfo GetRowInfo(uint key, uint key2 = 0, uint key3 = 0);

    public bool TryGetRowInfo(out NexRowInfo rowInfo, uint key, uint key2 = 0, uint key3 = 0);

    public List<NexRowInfo> GetAllRowInfos();
}
