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
    private Dictionary<uint, NexDoubleKeyedSet> _dkSets { get; set; } = [];
    private List<NexRowInfo> _allRows { get; set; } = [];

    public void Read(BinaryStream bs)
    {
        long basePos = bs.Position;

        uint thisHeaderSize = bs.ReadUInt32();
        uint setCount = bs.ReadUInt32();
        uint allRowsInfoOffset = bs.ReadUInt32();
        uint totalRowCount = bs.ReadUInt32();
        bs.ReadUInt32();

        for (int i = 0; i < setCount; i++)
        {
            bs.Position = basePos + thisHeaderSize + (i * 0x14);
            long rowSetInfoOffset = bs.Position;

            uint rowId = bs.ReadUInt32();
            uint rowSubSetOffset = bs.ReadUInt32();
            uint numSets = bs.ReadUInt32();
            uint unkOffset = bs.ReadUInt32();
            uint unkCount = bs.ReadUInt32();

            _dkSets.Add(rowId, new NexDoubleKeyedSet());

            for (int j = 0; j < numSets; j++)
            {
                bs.Position = rowSetInfoOffset + rowSubSetOffset + (j * 0x14);
                long rowSubSetInfoOffset = bs.Position;

                uint subId_ = bs.ReadUInt32();
                uint unkOffset_ = bs.ReadUInt32();
                uint unkCount_ = bs.ReadUInt32();
                uint rowInfosOffset = bs.ReadUInt32();
                uint arrayLength = bs.ReadUInt32();

                _dkSets[rowId].SubSets.TryAdd(subId_, []);

                for (int k = 0; k < arrayLength; k++)
                {
                    bs.Position = rowSubSetInfoOffset + rowInfosOffset + (k * 0x14);
                    long rowInfoOffset = bs.Position;

                    uint actualRowId = bs.ReadUInt32();
                    uint actualSubId = bs.ReadUInt32();
                    uint actualArrayIndex = bs.ReadUInt32();
                    bs.ReadUInt32(); // Unk
                    uint rowDataOffset = bs.ReadUInt32();

                    var rowInfo = new NexRowInfo(actualRowId, actualSubId, actualArrayIndex);
                    rowInfo.RowDataOffset = (int)(rowInfoOffset + rowDataOffset);

                    _dkSets[rowId].SubSets[actualSubId].Add(actualArrayIndex, rowInfo);
                }
            }
        }

        bs.Position = allRowsInfoOffset;
        for (int i = 0; i < totalRowCount; i++)
        {
            long rowInfoOffset = bs.Position;

            uint rowId = bs.ReadUInt32();
            uint subId = bs.ReadUInt32();
            uint arrayIndex = bs.ReadUInt32();
            bs.ReadUInt32(); // Always 0
            uint rowOffsetRelative = bs.ReadUInt32();

            var rowInfo = new NexRowInfo(rowId, subId, arrayIndex);
            rowInfo.RowDataOffset = (int)(rowInfoOffset + rowOffsetRelative);
            _allRows.Add(rowInfo);
        }
    }

    public NexRowInfo GetRowInfo(uint rowId, uint subId = 0, uint arrayIndex = 0)
    {
        if (!_dkSets.TryGetValue(rowId, out NexDoubleKeyedSet dkSet))
            throw new KeyNotFoundException($"RowId {rowId} does not exist in table.");

        if (!dkSet.SubSets.TryGetValue(subId, out Dictionary<uint, NexRowInfo> subSet))
            throw new KeyNotFoundException($"SubId {subId} for RowId {rowId} does not exist in table.");

        if (!subSet.TryGetValue(arrayIndex, out NexRowInfo rowInfo))
            throw new KeyNotFoundException($"Array index {arrayIndex} for ids {rowId},{subId} does not exist in table.");

        return rowInfo;
    }

    public Dictionary<uint, NexDoubleKeyedSet> GetRowSets() => _dkSets;

    public List<NexRowInfo> GetAllRowInfos()
    {
        return _allRows;
    }
}

public class NexDoubleKeyedSet
{
    public Dictionary<uint, Dictionary<uint, NexRowInfo>> SubSets { get; set; } = [];
}
