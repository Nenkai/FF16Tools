using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Nex.Entities;

public class NexRowInfo
{
    public uint Key { get; set; }
    public uint Key2 { get; set; }
    public uint Key3 { get; set; }
    public int RowDataOffset { get; set; }

    public NexRowInfo(uint key, uint key2 = 0, uint key3 = 0)
    {
        Key = key;
        Key2 = key2;
        Key3 = key3;
    }
}
