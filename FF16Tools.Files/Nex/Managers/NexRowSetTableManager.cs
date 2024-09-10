using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Nex.Managers;

public class NexRowSetTableManager : INexRowManager
{
    // Row length can be variable. See: battlehudgroup
    private Dictionary<uint, Dictionary<uint, NexRowInfo>> _rowSets { get; set; } = [];
    private List<NexRowInfo> _allRows { get; set; } = [];

    public void Read(BinaryStream bs)
    {
        long basePos = bs.Position;

        uint thisHeaderSize = bs.ReadUInt32();
        uint setCount = bs.ReadUInt32();
        bs.ReadUInt32();
        uint allRowsInfoOffset = bs.ReadUInt32();
        uint totalRowCount = bs.ReadUInt32();

        for (int i = 0; i < setCount; i++)
        {
            bs.Position = basePos + thisHeaderSize + (i * 0x0C);
            long rowSetInfoOffset = bs.Position;

            uint rowId = bs.ReadUInt32();
            uint rowArrayOffset = bs.ReadUInt32();
            uint arrayLength = bs.ReadUInt32();

            _rowSets.Add(rowId, []);

            for (int j = 0; j < arrayLength; j++)
            {
                bs.Position = rowSetInfoOffset + rowArrayOffset + (j * 0x0C);
                long rowInfoOffset = bs.Position;

                uint rowId_ = bs.ReadUInt32();
                uint arrayIndex = bs.ReadUInt32();
                uint rowDataOffsetRelative = bs.ReadUInt32();

                var rowInfo = new NexRowInfo(rowId, 0, arrayIndex);
                rowInfo.RowDataOffset = (int)(rowInfoOffset + rowDataOffsetRelative);

                if (!_rowSets.TryGetValue(rowId_, out Dictionary<uint, NexRowInfo> rowSet))
                {
                    rowSet = new Dictionary<uint, NexRowInfo>();
                    _rowSets.Add(rowId_, rowSet);
                }
                
                 if (!rowSet.TryAdd(arrayIndex, rowInfo))
                    throw new Exception($"Row at array index {arrayIndex} already exists in row set");
            }
        }

        bs.Position = allRowsInfoOffset;
        for (int i = 0; i < totalRowCount; i++)
        {
            long rowInfoOffset = bs.Position;

            uint rowId = bs.ReadUInt32();
            uint arrayIndex = bs.ReadUInt32();
            uint rowOffsetRelative = bs.ReadUInt32();

            var rowInfo = new NexRowInfo(rowId, 0, arrayIndex);
            rowInfo.RowDataOffset = (int)(rowInfoOffset + rowOffsetRelative);
            _allRows.Add(rowInfo);
        }
    }

    public NexRowInfo GetRowInfo(uint rowId, uint subId = 0, uint arrayIndex = 0)
    {
        if (!_rowSets.TryGetValue(rowId, out Dictionary<uint, NexRowInfo> rowSet))
            throw new Exception($"Row set {rowId} was not found.");

        if (!rowSet.TryGetValue(subId, out NexRowInfo rowInfo))
            throw new Exception($"Array index {arrayIndex} for row set {rowId} was not found.");

        return rowInfo;
    }

    public Dictionary<uint, Dictionary<uint, NexRowInfo>> GetRowSets() => _rowSets;

    public List<NexRowInfo> GetAllRowInfos()
    {
        return _allRows;
    }
}
