using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Nex.Entities;

public class NexTableLayout
{
    /// <summary>
    /// Game Codename for this table/sheet.
    /// </summary>
    public string CodeName { get; set; }

    /// <summary>
    /// Name of the table/sheet.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Type of table/sheet.
    /// </summary>
    public NexTableType Type { get; set; }

    /// <summary>
    /// Localization category for this table/sheet.
    /// </summary>
    public NexTableCategory Category { get; set; }

    /// <summary>
    /// Whether to use base row id. If the base row id is 1000 and the game fetches 1, it is actually 1001.
    /// </summary>
    public bool UsesBaseRowId { get; set; }

    /// <summary>
    /// Size of one row, inline, without arrays.
    /// </summary>
    public int TotalInlineSize { get; set; }

    /// <summary>
    /// Table/sheet columns.
    /// </summary>
    public Dictionary<string, NexStructColumn> Columns { get; set; } = [];

    /// <summary>
    /// Custom struct definitions for this table/sheet.
    /// </summary>
    public Dictionary<string, NexTableColumnStruct> CustomStructDefinitions { get; set; } = [];

    /// <summary>
    /// Comments for specific rows, used to fill the 'Comment' column.
    /// </summary>
    public Dictionary<(uint, uint, uint), string> RowComments { get; set; } = [];

    public NexTableLayout(string codeName)
    {
        CodeName = codeName;
    }
}
