using FF16Tools.Files.Nex.Entities;
using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Nex.Managers;

public class NexRowTableManager : INexRowManager
{
    // Row length can be variable. See: battlehudgroup
    public Dictionary<uint, NexRowInfo> _rows { get; set; } = [];

    public void Read(BinaryStream bs)
    {
        uint rowInfosOffset = bs.ReadUInt32();
        uint numRows = bs.ReadUInt32();

        List<(uint RowId, int RowDataOffset)> rowInfos = [];
        for (int i = 0; i < numRows; i++)
        {
            bs.Position = rowInfosOffset + i * 0x08;

            long currentPos = bs.Position;
            uint rowId = bs.ReadUInt32();
            uint rowDataOffsetRelative = bs.ReadUInt32();

            rowInfos.Add((rowId, (int)(currentPos + rowDataOffsetRelative)));
        }

        for (int i = 0; i < numRows; i++)
        {
            bs.Position = rowInfos[i].RowDataOffset;
            _rows.Add(rowInfos[i].RowId, new NexRowInfo(rowInfos[i].RowId) { RowDataOffset = rowInfos[i].RowDataOffset });
        }
    }

    public NexRowInfo GetRowInfo(uint rowId, uint subId = 0, uint arrayIndex = 0)
    {
        if (!_rows.TryGetValue(rowId, out NexRowInfo rowInfo))
            throw new Exception($"Row id {rowId} was not found.");

        return rowInfo;
    }

    public List<NexRowInfo> GetAllRowInfos()
    {
        return _rows.Values.ToList();
    }
}
