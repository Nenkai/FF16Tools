using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Syroot.BinaryData;

using FF16Tools.Files.Nex.Entities;

namespace FF16Tools.Files.Nex;

/// <summary>
/// Nex (nxd) file builder.
/// </summary>
public class NexDataFileBuilder
{
    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    public NexTableType Type { get; }
    public NexTableCategory Category { get; }
    public bool UsesBaseRow { get; }

    private Dictionary<(uint Key, uint Key2, uint Key3), NexRowBuild> _rows = [];
    private SortedDictionary<uint, SortedList<uint, NexRowBuild>> _rowSets = [];
    private SortedDictionary<uint, SortedList<uint, SortedList<uint, NexRowBuild>>> _dkSets = [];

    private NexTableLayout _columnLayout;

    private record ArrayPointerRef(int RelativeOffset, int StringFieldOffset);
    private Dictionary<byte[], List<ArrayPointerRef>> _byteArrayTable = new(new ByteArrayComparer());

    private long _lastRowDataStartOffset;
    private long _lastDataEndOffset;

    public NexDataFileBuilder(NexTableLayout columnLayout, 
        ILoggerFactory loggerFactory = null)
    {
        ArgumentNullException.ThrowIfNull(columnLayout, nameof(columnLayout));

        if (!Enum.IsDefined(columnLayout.Type))
            throw new ArgumentException("Invalid nex table type.");

        if (!Enum.IsDefined(columnLayout.Category))
            throw new ArgumentException("Invalid nex category type.");

        Type = columnLayout.Type;
        Category = columnLayout.Category;
        UsesBaseRow = columnLayout.UsesBaseRowId;

        _columnLayout = columnLayout;

        _loggerFactory = loggerFactory;
        if (_loggerFactory is not null)
            _logger = _loggerFactory.CreateLogger(GetType().ToString());
    }

    /// <summary>
    /// Adds a double keyed set to the builder (table type must be <see cref="NexTableType.TripleKeyed"/>).
    /// </summary>
    /// <param name="rowId"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void AddTripleKeyedSet(uint rowId)
    {
        if (Type == NexTableType.TripleKeyed)
            _dkSets.TryAdd(rowId, []);
        else
            throw new InvalidOperationException("Trying to add a triple-keyed set to a non triple-keyed row table.");
    }

    /// <summary>
    /// Adds a row sub set to the builder (table type must be <see cref="NexTableType.TripleKeyed"/>).
    /// </summary>
    /// <param name="key1"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void AddTripleKeyedSubset(uint key1, uint key2)
    {
        if (Type == NexTableType.TripleKeyed)
        {
            AddTripleKeyedSet(key1);
            _dkSets[key1].TryAdd(key2, []);
        }
        else
            throw new InvalidOperationException("Trying to add a triple-keyed sub set to a non triple-keyed row table.");
    }

    /// <summary>
    /// Adds a row set to the builder (table type must be <see cref="NexTableType.DoubleKeyed"/>).
    /// </summary>
    /// <param name="rowId"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void AddDoubleKeyedSet(uint keyId)
    {
        if (Type == NexTableType.DoubleKeyed)
            _rowSets.TryAdd(keyId, []);
        else
            throw new InvalidOperationException("Trying to add a doubled-keyed set to a non double-keyed table.");
    }

    /// <summary>
    /// Adds a row to the builder. Sets will be created if they do not already exist.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="key2"></param>
    /// <param name="key3"></param>
    /// <param name="cells">Row cells.</param>
    /// <param name="overwriteIfExists">Whether to overwrite if the row exists (will still return false). If this is false, an exception will be thrown instead.</param>
    public bool AddRow(uint key, uint key2, uint key3, List<object> cells, bool overwriteIfExists = false)
    {
        ArgumentNullException.ThrowIfNull(cells, nameof(cells));

        if (cells.Count != _columnLayout.Columns.Count)
            throw new ArgumentException($"Cells only has {cells.Count} element(s), while the layout expects {_columnLayout.Columns.Count} columns.");

        var row = new NexRowBuild(key, key2, key3, cells);

        if (Type == NexTableType.DoubleKeyed)
        {
            AddDoubleKeyedSet(key);

            if (!_rowSets[key].TryAdd(row.Key2, row))
            {
                if (overwriteIfExists)
                    _rowSets[key][row.Key2] = row;
                else
                    throw new Exception($"Row with key ({key},{key2},{key3}) already exists in builder.");
            }
        }
        else if (Type == NexTableType.TripleKeyed)
        {
            AddTripleKeyedSubset(key, key2);

            if (!_dkSets[key][key2].TryAdd(row.Key3, row))
            {
                if (overwriteIfExists)
                    _dkSets[key][key2][key3] = row;
                else
                    throw new Exception($"Row with key ({key},{key2},{key3}) already exists in builder.");
            }
        }

        bool res = _rows.TryAdd((key, key2, key3), row);
        if (!res)
        {
            if (overwriteIfExists)
                _rows[(key, key2, key3)] = row;
            else
                throw new Exception($"Row with key ({key},{key2},{key3}) already exists in builder.");

            return false;
        }

        return true;
    }

    /// <summary>
    /// Removes a row from the builder.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="key2"></param>
    /// <param name="key3"></param>
    public bool RemoveRow(uint key, uint key2, uint key3)
    {
        if (Type == NexTableType.DoubleKeyed)
        {
            _rowSets[key].Remove(key2);
            if (_rowSets[key].Count == 0)
                _rowSets.Remove(key);
        }
        else if (Type == NexTableType.TripleKeyed)
        {
            _dkSets[key][key2].Remove(key3);
            if (_dkSets[key][key2].Count == 0)
            {
                _dkSets[key].Remove(key2);
                if (_dkSets[key].Count == 0)
                    _dkSets.Remove(key);
            }
        }

        return _rows.Remove((key, key2, key3));
    }

    /// <summary>
    /// Gets a row from the builder (for cell edit only).
    /// </summary>
    /// <param name="key"></param>
    /// <param name="key2"></param>
    /// <param name="key3"></param>
    /// <returns></returns>
    public NexRowBuild GetRow(uint key, uint key2, uint key3)
    {
        return _rows[(key, key2, key3)];
    }

    public void Write(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream, nameof(stream));

        uint baseRowId = 0;
        if (UsesBaseRow && _rows.Count > 0)
            baseRowId = _rows.First().Key.Key;

        var bs = new BinaryStream(stream, ByteConverter.Little);
        bs.WriteUInt32(NexDataFile.MAGIC);
        bs.WriteUInt32(1);
        bs.WriteByte((byte)Type);
        bs.WriteByte((byte)Category);
        bs.WriteBoolean(UsesBaseRow);
        bs.WriteByte(0);
        bs.WriteUInt32(baseRowId); // base row id
        bs.Position += 0x10;

        switch (Type)
        {
            case NexTableType.SingleKeyed:
                WriteRowTable(bs);
                break;

            case NexTableType.DoubleKeyed:
                WriteDoubleKeyedTable(bs);
                break;

            case NexTableType.TripleKeyed:
                WriteTripleKeyedRowTable(bs);
                break;

            default:
                throw new NotSupportedException($"Wrong nex table type '{Type}'? Unable to serialize.");
        }
    }

    private void WriteRowTable(BinaryStream bs)
    {
        _rows = _rows.OrderBy(e => e.Key.Key).ToDictionary();

        bs.WriteUInt32(0x30); // offset to rows
        bs.WriteUInt32((uint)_rows.Count);
        bs.Align(0x10, grow: true);

        long rowInfosOffset = bs.Position;
        _lastRowDataStartOffset = bs.Position + (_rows.Count * 0x08);
        _lastDataEndOffset = _lastRowDataStartOffset;

        int i = 0;
        foreach (NexRowBuild row in _rows.Values)
        {
            _lastRowDataStartOffset = _lastDataEndOffset;
            _lastDataEndOffset = _lastRowDataStartOffset + _columnLayout.TotalInlineSize;

            int thisRowInfoOffset = (int)rowInfosOffset + (i * 0x08);
            bs.Position = thisRowInfoOffset;
            bs.WriteUInt32(row.Key);
            bs.WriteUInt32((uint)(_lastRowDataStartOffset - thisRowInfoOffset));

            bs.Position = _lastRowDataStartOffset;
            WriteRowData(bs, row);

            i++;
        }

        WriteByteArrayTable(bs);
    }

    private void WriteDoubleKeyedTable(BinaryStream bs)
    {
        _rows = _rows.OrderBy(e => e.Key.Key)
                     .ThenBy(e => e.Key.Key2).ToDictionary();

        long subHeaderOffset = bs.Position;
        bs.WriteUInt32(0x1C); // offset to rows
        bs.WriteInt32(_rowSets.Count);
        bs.WriteInt32(0);
        bs.WriteUInt32(0); // rows info offset (write later)
        bs.WriteInt32(_rows.Count);
        bs.WriteBytes(new byte[8]); // Pad

        long rowSetInfosOffset = bs.Position;
        _lastDataEndOffset = rowSetInfosOffset + (_rowSets.Count * 0x0C);

        int i = 0;
        foreach (var set in _rowSets)
        {
            long thisRowSetInfoOffset = _lastDataEndOffset;
            _lastRowDataStartOffset = thisRowSetInfoOffset + (set.Value.Count * 0x0C);
            _lastDataEndOffset = _lastRowDataStartOffset;

            long arrOffset = rowSetInfosOffset + (i * 0x0C);
            bs.Position = arrOffset;
            bs.WriteUInt32(set.Key);
            bs.WriteUInt32((uint)(thisRowSetInfoOffset - arrOffset));
            bs.WriteUInt32((uint)set.Value.Count);

            int j = 0;
            foreach (var arr in set.Value)
            {
                NexRowBuild row = arr.Value;

                _lastRowDataStartOffset = _lastDataEndOffset;
                _lastDataEndOffset = _lastRowDataStartOffset + _columnLayout.TotalInlineSize;

                int thisRowInfoOffset = (int)thisRowSetInfoOffset + (j * 0x0C);
                bs.Position = thisRowInfoOffset;
                bs.WriteUInt32(row.Key);
                bs.WriteUInt32(row.Key2);
                bs.WriteUInt32((uint)(_lastRowDataStartOffset - thisRowInfoOffset));

                bs.Position = _lastRowDataStartOffset;
                WriteRowData(bs, row);
                j++;
            }

            i++;
        }

        long rowDataInfoOffset = _lastDataEndOffset;
        i = 0;
        foreach (var row in _rows.Values)
        {
            long thisRowInfoOffset = rowDataInfoOffset + (i * 0x0C);
            bs.Position = thisRowInfoOffset;

            bs.WriteUInt32(row.Key);
            bs.WriteUInt32(row.Key2);
            bs.WriteInt32((int)(row.RowDataOffset - thisRowInfoOffset)); // Will be negative

            i++;
        }
        _lastDataEndOffset = bs.Position;
        WriteByteArrayTable(bs);

        long tempLastPos = bs.Position;

        bs.Position = subHeaderOffset + 0x0C;
        bs.WriteUInt32((uint)rowDataInfoOffset);

        bs.Position = tempLastPos;
    }

    private void WriteTripleKeyedRowTable(BinaryStream bs)
    {
        _rows = _rows.OrderBy(e => e.Key.Key)
                .ThenBy(e => e.Key.Key2)
                .ThenBy(e => e.Key.Key3).ToDictionary();

        long subHeaderOffset = bs.Position;
        bs.WriteUInt32(0x18); // offset to rows
        bs.WriteInt32(_dkSets.Count);
        bs.WriteUInt32(0); // rows info offset (write later)
        bs.WriteInt32(_rows.Count);
        bs.WriteBytes(new byte[8]); // Pad

        long dkSetInfosOffset = bs.Position;
        _lastRowDataStartOffset = dkSetInfosOffset + (_dkSets.Count * 0x14);

        int i = 0;
        foreach (var dkSet in _dkSets)
        {
            long thisDkSetInfoOffset = dkSetInfosOffset + (i * 0x14);
            long subSetsInfoOffset = _lastRowDataStartOffset;

            bs.Position = thisDkSetInfoOffset;
            bs.WriteUInt32(dkSet.Key);
            bs.WriteUInt32((uint)(subSetsInfoOffset - thisDkSetInfoOffset));
            bs.WriteUInt32((uint)dkSet.Value.Count);
            bs.WriteUInt32(0); // Unk offset (writen after)
            bs.WriteUInt32(0); // Unk count (always 0)

            _lastRowDataStartOffset = subSetsInfoOffset + (dkSet.Value.Count * 0x14);
            _lastDataEndOffset = _lastRowDataStartOffset;

            int j = 0;
            foreach (var subset in dkSet.Value)
            {
                int thisSubSetInfoOffset = (int)subSetsInfoOffset + (j * 0x14);
                bs.Position = thisSubSetInfoOffset;
                bs.WriteUInt32(subset.Key);
                bs.WriteUInt32((uint)(_lastRowDataStartOffset - thisSubSetInfoOffset)); // unk (probably an offset to something else. for now just write the same offset)
                bs.WriteUInt32(0); // Unk count
                bs.WriteUInt32((uint)(_lastRowDataStartOffset - thisSubSetInfoOffset));
                bs.WriteUInt32((uint)subset.Value.Count); // Num rows

                long rowInfosOffset = _lastRowDataStartOffset;
                _lastRowDataStartOffset = rowInfosOffset + (subset.Value.Count * 0x14);
                _lastDataEndOffset = _lastRowDataStartOffset;

                int k = 0;
                foreach (var actual in subset.Value)
                {
                    _lastRowDataStartOffset = _lastDataEndOffset;
                    _lastDataEndOffset = _lastRowDataStartOffset + _columnLayout.TotalInlineSize;

                    long thisRowInfoOffset = rowInfosOffset + (k * 0x14);
                    bs.Position = thisRowInfoOffset;

                    NexRowBuild row = actual.Value;
                    bs.WriteUInt32(row.Key);
                    bs.WriteUInt32(row.Key2);
                    bs.WriteUInt32(row.Key3);
                    bs.WriteUInt32(0);
                    bs.WriteUInt32((uint)(_lastRowDataStartOffset - thisRowInfoOffset));

                    bs.Position = _lastRowDataStartOffset;
                    WriteRowData(bs, row);
                    k++;

                    _lastRowDataStartOffset = _lastDataEndOffset;
                }

                j++;
            }

            bs.Position = thisDkSetInfoOffset + 0x0C;
            bs.WriteUInt32((uint)(_lastDataEndOffset - thisDkSetInfoOffset));

            i++;
        }

        long rowDataInfoOffset = _lastDataEndOffset;
        i = 0;
        foreach (var row in _rows.Values)
        {
            long thisRowInfoOffset = rowDataInfoOffset + (i * 0x14);
            bs.Position = thisRowInfoOffset;

            bs.WriteUInt32(row.Key);
            bs.WriteUInt32(row.Key2);
            bs.WriteUInt32(row.Key3);
            bs.WriteUInt32(0);
            bs.WriteInt32((int)(row.RowDataOffset - thisRowInfoOffset)); // Will be negative

            i++;
        }
        _lastDataEndOffset = bs.Position;
        WriteByteArrayTable(bs);

        long tempLastPos = bs.Position;

        bs.Position = subHeaderOffset + 0x08;
        bs.WriteUInt32((uint)rowDataInfoOffset);

        bs.Position = tempLastPos;
    }

    private void WriteRowData(BinaryStream bs, NexRowBuild row)
    {
        row.RowDataOffset = (int)bs.Position;

        int j = 0;
        foreach (NexStructColumn column in _columnLayout.Columns.Values)
        {
            bs.Position = row.RowDataOffset + (int)column.Offset;
            WriteCell(bs, (int)_lastRowDataStartOffset, row.Cells[j], column);
            j++;
        }

        j = 0;
        foreach (NexStructColumn column in _columnLayout.Columns.Values)
        {
            object obj = row.Cells[j++];
            if (obj is not Array)
                continue;

            WriteArray(bs, (int)_lastRowDataStartOffset, (int)_lastRowDataStartOffset + (int)column.Offset, obj, column);
        }

        bs.Align(0x04, grow: true);
    }

    private void WriteCell(BinaryStream bs, int rowDataOffset, object cellValue, NexStructColumn column)
    {
        switch (column.Type)
        {
            case NexColumnType.SByte:
                bs.WriteSByte((sbyte)cellValue);
                break;
            case NexColumnType.Byte:
                bs.WriteByte((byte)cellValue);
                break;
            case NexColumnType.Short:
                bs.WriteInt16((short)cellValue); 
                break;
            case NexColumnType.UShort:
                bs.WriteUInt16((ushort)cellValue);
                break;
            case NexColumnType.Int:
                bs.WriteInt32((int)cellValue);
                break;
            case NexColumnType.UInt:
            case NexColumnType.HexUInt:
                bs.WriteUInt32((uint)cellValue);
                break;
            case NexColumnType.Float:
                bs.WriteSingle((float)cellValue);
                break;

            case NexColumnType.Union:
                {
                    NexUnion union = (NexUnion)cellValue;
                    bs.WriteUInt16((ushort)union.Type);
                    bs.WriteUInt16(0); // align
                    bs.WriteInt32(union.Value);
                }
                break;
            case NexColumnType.ByteArray: // byte arrays are combined with strings at the end of the file
                {
                    int relOffset = column.UsesRelativeOffset ? (int)bs.Position + column.RelativeOffsetShift : rowDataOffset;
                    byte[] array = (byte[])cellValue;
                    AddByteArray(bs, relOffset, array);
                    bs.Position += 8; // Skip (write later)
                }
                break;

            case NexColumnType.String:
                {
                    int relOffset = column.UsesRelativeOffset ? (int)bs.Position + column.RelativeOffsetShift : rowDataOffset;
                    AddString(bs, relOffset, cellValue);
                    bs.Position += 4; // Skip (write later)
                    break;
                }

            case NexColumnType.IntArray:
            case NexColumnType.UIntArray:
            case NexColumnType.FloatArray:
            case NexColumnType.StringArray:
            case NexColumnType.CustomStructArray:
            case NexColumnType.UnionArray:
                {
                    bs.Position += 8; // Skip (write later)
                }
                break;

            default:
                throw new NotImplementedException();
        }

        if (bs.Position > _lastDataEndOffset)
            _lastDataEndOffset = bs.Position;
    }

    private void WriteArray(BinaryStream bs, int rowDataOffset, int arrayFieldOffset, object cellValue, NexStructColumn column)
    {
        bs.Position = _lastDataEndOffset;
        int arrayDataOffset = (int)_lastDataEndOffset;

        int arrayLength;
        switch (column.Type)
        {
            case NexColumnType.ByteArray:
                {
                    byte[] array = (byte[])cellValue;
                    arrayLength = array.Length;
                    break;
                }

            case NexColumnType.IntArray:
                {
                    int[] array = (int[])cellValue;
                    for (int i = 0; i < array.Length; i++)
                        bs.WriteInt32(array[i]);
                    arrayLength = array.Length;
                    break;
                }
            case NexColumnType.UIntArray:
                {
                    uint[] array = (uint[])cellValue;
                    for (int i = 0; i < array.Length; i++)
                        bs.WriteUInt32(array[i]);
                    arrayLength = array.Length;
                    break;
                }
            case NexColumnType.FloatArray:
                {
                    float[] array = (float[])cellValue;
                    for (int i = 0; i < array.Length; i++)
                        bs.WriteSingle(array[i]);
                    arrayLength = array.Length;
                    break;
                }
            case NexColumnType.UnionArray:
                {
                    NexUnion[] array = (NexUnion[])cellValue;
                    for (int i = 0; i < array.Length; i++)
                    {
                        bs.WriteUInt16((ushort)array[i].Type);
                        bs.WriteUInt16(0);
                        bs.WriteInt32(array[i].Value);
                    }
                    arrayLength = array.Length;
                    break;
                }
            case NexColumnType.StringArray:
                {
                    string[] array = (string[])cellValue;
                    for (int i = 0; i < array.Length; i++)
                    {
                        AddString(bs, arrayDataOffset, array[i]);
                        bs.Position += 0x04;
                    }

                    arrayLength = array.Length;
                    break;
                }
            case NexColumnType.CustomStructArray:
                {
                    object[] array = (object[])cellValue;
                    List<int> arrayOffsets = new(array.Length);
                    for (int i = 0; i < array.Length; i++)
                    {
                        int arrayOffset = (int)bs.Position;
                        arrayOffsets.Add(arrayOffset);
                        object[] structFields = (object[])array[i];
                        NexTableColumnStruct customStruct = _columnLayout.CustomStructDefinitions[column.StructTypeName];

                        for (int j = 0; j < customStruct.Columns.Count; j++)
                        {
                            NexStructColumn field = customStruct.Columns[j];
                            bs.Position = arrayOffset + field.Offset;
                            WriteCell(bs, arrayOffset, structFields[j], field);
                        }

                        // Important to explicitly pad the struct.
                        bs.Align(0x04, grow: true);
                    }

                    // Sub-Arrays are always after
                    for (int i = 0; i < array.Length; i++)
                    {
                        object[] structFields = (object[])array[i];
                        NexTableColumnStruct customStruct = _columnLayout.CustomStructDefinitions[column.StructTypeName];

                        for (int j = 0; j < customStruct.Columns.Count; j++)
                        {
                            NexStructColumn field = customStruct.Columns[j];
                            object obj = structFields[j];
                            if (obj is not Array)
                                continue;

                            WriteArray(bs, arrayOffsets[i], arrayOffsets[i] + (int)field.Offset, obj, field);
                        }
                    }
                    arrayLength = array.Length;
                    break;
                };

            default:
                throw new NotImplementedException();
        }

        _lastDataEndOffset = bs.Position;

        bs.Position = arrayFieldOffset;
        if (column.UsesRelativeOffset)
            bs.WriteInt32(arrayDataOffset - (arrayFieldOffset + column.RelativeOffsetShift));
        else
            bs.WriteInt32(arrayDataOffset - rowDataOffset);

        bs.WriteInt32(arrayLength);

        bs.Position = _lastDataEndOffset;
    }

    private void AddString(BinaryStream bs, int rowDataOffset, object cellValue)
    {
        string str = cellValue as string ?? string.Empty; // Sanity check

        byte[] byteArr = Encoding.UTF8.GetBytes(str + '\0');
        AddByteArray(bs, rowDataOffset, byteArr);
    }

    private void AddByteArray(BinaryStream bs, int rowDataOffset, byte[] byteArr)
    {
        ArrayPointerRef byteArrRef = new(rowDataOffset, (int)bs.Position);
        if (_byteArrayTable.TryGetValue(byteArr, out List<ArrayPointerRef> byteArrayRefs))
            byteArrayRefs.Add(byteArrRef);
        else
            _byteArrayTable.Add(byteArr, [byteArrRef]);
    }

    private void WriteByteArrayTable(BinaryStream bs)
    {
        long lastArrOffset = _lastDataEndOffset;
        foreach (var array in _byteArrayTable)
        { 
            foreach (ArrayPointerRef byteArrayRef in array.Value)
            {
                bs.Position = byteArrayRef.StringFieldOffset;
                int actualDataOffset = (int)lastArrOffset - byteArrayRef.RelativeOffset;
                bs.WriteInt32(actualDataOffset);
            }

            bs.Position = lastArrOffset;
            bs.Write(array.Key);
            lastArrOffset = bs.Position;
        }

        _lastDataEndOffset = lastArrOffset;
    }
}

public class NexRowBuild
{
    public uint Key { get; }
    public uint Key2 { get; }
    public uint Key3 { get; }
    public List<object> Cells { get; }
    public int RowDataOffset { get; set; }

    public NexRowBuild(uint key, uint key2, uint key3, List<object> cells)
    {
        Key = key;
        Key2 = key2;
        Key3 = key3;
        Cells = cells;
    }

    public override string ToString()
    {
        return $"Key: {Key}, Key2: {Key2}, Key3: {Key3}";
    }
}

public class ByteArrayComparer : IEqualityComparer<byte[]>
{
    public bool Equals(byte[] left, byte[] right)
    {
        if (left == null || right == null)
        {
            return left == right;
        }
        if (left.Length != right.Length)
        {
            return false;
        }
        for (int i = 0; i < left.Length; i++)
        {
            if (left[i] != right[i])
            {
                return false;
            }
        }
        return true;
    }
    public int GetHashCode(byte[] key)
    {
        if (key == null)
            throw new ArgumentNullException("key");
        int sum = 0;
        foreach (byte cur in key)
        {
            sum += cur;
        }
        return sum;
    }
}