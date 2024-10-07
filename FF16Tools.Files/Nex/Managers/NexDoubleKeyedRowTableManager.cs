using FF16Tools.Files.Nex.Entities;
using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Nex.Managers;

public class NexDoubleKeyedRowTableManager : INexRowManager
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

            uint key = bs.ReadUInt32();
            uint rowArrayOffset = bs.ReadUInt32();
            uint arrayLength = bs.ReadUInt32();

            _rowSets.Add(key, []);

            for (int j = 0; j < arrayLength; j++)
            {
                bs.Position = rowSetInfoOffset + rowArrayOffset + (j * 0x0C);
                long rowInfoOffset = bs.Position;

                uint key_ = bs.ReadUInt32();
                uint key2 = bs.ReadUInt32();
                uint rowDataOffsetRelative = bs.ReadUInt32();

                var rowInfo = new NexRowInfo(key, key2, 0);
                rowInfo.RowDataOffset = (int)(rowInfoOffset + rowDataOffsetRelative);

                if (!_rowSets.TryGetValue(key_, out Dictionary<uint, NexRowInfo> rowSet))
                {
                    rowSet = new Dictionary<uint, NexRowInfo>();
                    _rowSets.Add(key_, rowSet);
                }
                
                 if (!rowSet.TryAdd(key2, rowInfo))
                    throw new Exception($"Row at array index {key2} already exists in row set");
            }
        }

        bs.Position = allRowsInfoOffset;
        for (int i = 0; i < totalRowCount; i++)
        {
            long rowInfoOffset = bs.Position;

            uint key = bs.ReadUInt32();
            uint key2 = bs.ReadUInt32();
            uint rowOffsetRelative = bs.ReadUInt32();

            var rowInfo = new NexRowInfo(key, key2, 0);
            rowInfo.RowDataOffset = (int)(rowInfoOffset + rowOffsetRelative);
            _allRows.Add(rowInfo);
        }
    }

    public NexRowInfo GetRowInfo(uint key, uint key2 = 0, uint key3 = 0)
    {
        if (!_rowSets.TryGetValue(key, out Dictionary<uint, NexRowInfo> rowSet))
            throw new Exception($"Key {key} was not found.");

        if (!rowSet.TryGetValue(key2, out NexRowInfo rowInfo))
            throw new Exception($"Key2 {key2} for row set key {key} was not found.");

        return rowInfo;
    }

    public bool TryGetRowInfo(out NexRowInfo rowInfo, uint key, uint key2 = 0, uint key3 = 0)
    {
        rowInfo = default;

        if (!_rowSets.TryGetValue(key, out Dictionary<uint, NexRowInfo> rowSet))
            return false;

        if (!rowSet.TryGetValue(key2, out rowInfo))
            return false;

        return true;
    }

    public Dictionary<uint, Dictionary<uint, NexRowInfo>> GetRowSets() => _rowSets;

    public List<NexRowInfo> GetAllRowInfos()
    {
        return _allRows;
    }
}
