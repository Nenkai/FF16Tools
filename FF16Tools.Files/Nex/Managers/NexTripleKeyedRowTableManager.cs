using FF16Tools.Files.Nex.Entities;
using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Nex.Managers;

public class NexTripleKeyedRowTableManager : INexRowManager
{
    private Dictionary<uint, NexTripleKeyedSet> _tkSets { get; set; } = [];
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

            uint key_ = bs.ReadUInt32();
            uint rowSubSetOffset = bs.ReadUInt32();
            uint numSets = bs.ReadUInt32();
            uint unkOffset = bs.ReadUInt32();
            uint unkCount = bs.ReadUInt32();

            _tkSets.Add(key_, new NexTripleKeyedSet());

            for (int j = 0; j < numSets; j++)
            {
                bs.Position = rowSetInfoOffset + rowSubSetOffset + (j * 0x14);
                long rowSubSetInfoOffset = bs.Position;

                uint subId_ = bs.ReadUInt32();
                uint unkOffset_ = bs.ReadUInt32();
                uint unkCount_ = bs.ReadUInt32();
                uint rowInfosOffset = bs.ReadUInt32();
                uint arrayLength = bs.ReadUInt32();

                _tkSets[key_].SubSets.TryAdd(subId_, []);

                for (int k = 0; k < arrayLength; k++)
                {
                    bs.Position = rowSubSetInfoOffset + rowInfosOffset + (k * 0x14);
                    long rowInfoOffset = bs.Position;

                    uint key = bs.ReadUInt32();
                    uint key2 = bs.ReadUInt32();
                    uint key3 = bs.ReadUInt32();
                    bs.ReadUInt32(); // Unk
                    uint rowDataOffset = bs.ReadUInt32();

                    var rowInfo = new NexRowInfo(key, key2, key3);
                    rowInfo.RowDataOffset = (int)(rowInfoOffset + rowDataOffset);

                    _tkSets[key_].SubSets[key2].Add(key3, rowInfo);
                }
            }
        }

        bs.Position = allRowsInfoOffset;
        for (int i = 0; i < totalRowCount; i++)
        {
            long rowInfoOffset = bs.Position;

            uint key = bs.ReadUInt32();
            uint key2 = bs.ReadUInt32();
            uint key3 = bs.ReadUInt32();
            bs.ReadUInt32(); // Always 0
            uint rowOffsetRelative = bs.ReadUInt32();

            var rowInfo = new NexRowInfo(key, key2, key3);
            rowInfo.RowDataOffset = (int)(rowInfoOffset + rowOffsetRelative);
            _allRows.Add(rowInfo);
        }
    }

    public NexRowInfo GetRowInfo(uint key, uint key2 = 0, uint key3 = 0)
    {
        if (!_tkSets.TryGetValue(key, out NexTripleKeyedSet? dkSet))
            throw new KeyNotFoundException($"Key {key} does not exist in table.");

        if (!dkSet.SubSets.TryGetValue(key2, out Dictionary<uint, NexRowInfo>? subSet))
            throw new KeyNotFoundException($"Key2 {key2} for Key {key} does not exist in table.");

        if (!subSet.TryGetValue(key3, out NexRowInfo? rowInfo))
            throw new KeyNotFoundException($"Key 3 {key3} for Keys {key},{key2} does not exist in table.");

        return rowInfo;
    }

    public bool TryGetRowInfo([NotNullWhen(true)] out NexRowInfo? rowInfo, uint key, uint key2 = 0, uint key3 = 0)
    {
        rowInfo = default;

        if (!_tkSets.TryGetValue(key, out NexTripleKeyedSet? dkSet))
            return false;

        if (!dkSet.SubSets.TryGetValue(key2, out Dictionary<uint, NexRowInfo>? subSet))
            return false;

        if (!subSet.TryGetValue(key3, out rowInfo))
            return false;

        return true;
    }

    public Dictionary<uint, NexTripleKeyedSet> GetRowSets() => _tkSets;

    public List<NexRowInfo> GetAllRowInfos()
    {
        return _allRows;
    }
}

public class NexTripleKeyedSet
{
    public Dictionary<uint, Dictionary<uint, NexRowInfo>> SubSets { get; set; } = [];
}
