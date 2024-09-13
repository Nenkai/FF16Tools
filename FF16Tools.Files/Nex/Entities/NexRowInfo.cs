using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Nex.Entities;

public class NexRowInfo
{
    public uint Id { get; set; }
    public uint SubId { get; set; }
    public uint ArrayIndex { get; set; }
    public int RowDataOffset { get; set; }

    public NexRowInfo(uint id, uint subId = 0, uint arrayIndex = 0)
    {
        Id = id;
        SubId = subId;
        ArrayIndex = arrayIndex;
    }
}
