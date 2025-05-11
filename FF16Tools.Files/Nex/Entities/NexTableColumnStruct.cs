using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Nex.Entities;

/// <summary>
/// Represents a nested struct within a nex row.
/// </summary>
public class NexTableColumnStruct
{
    /// <summary>
    /// Name of the struct.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Columns/Fields for this struct.
    /// </summary>
    public List<NexStructColumn> Columns { get; set; } = [];

    /// <summary>
    /// Flat size of this struct.
    /// </summary>
    public int TotalInlineSize { get; set; }

    public NexTableColumnStruct(string name)
    {
        Name = name;
    }
}