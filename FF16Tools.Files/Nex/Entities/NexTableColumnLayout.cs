using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Nex.Entities;

public class NexTableColumnLayout
{
    public int TotalInlineSize { get; set; }
    public List<NexStructColumn> Columns { get; set; } = [];
    public Dictionary<string, List<NexStructColumn>> CustomStructDefinitions { get; set; } = [];
}
