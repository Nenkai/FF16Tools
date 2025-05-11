using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Nex.Entities;

public class NexTableLayout
{
    public NexTableType Type { get; set; }
    public NexTableCategory Category { get; set; }
    public bool UsesBaseRowId { get; set; }
    public int TotalInlineSize { get; set; }
    public Dictionary<string, NexStructColumn> Columns { get; set; } = [];
    public Dictionary<string, NexTableColumnStruct> CustomStructDefinitions { get; set; } = [];

    public Dictionary<(uint, uint, uint), string> RowComments { get; set; } = [];
}
