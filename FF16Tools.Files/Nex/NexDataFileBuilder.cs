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
using System.Collections;

namespace FF16Tools.Files.Nex;

public class NexDataFileBuilder
{
    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    public NexTableType Type { get; }
    public NexTableCategory Category { get; }
    public bool UsesBaseRow { get; }
    public uint BaseRowId { get; }

    private List<NexRowBuild> _rows = [];
    private Dictionary<uint, List<NexRowBuild>> _rowSets = [];

    private NexTableColumnLayout _columnLayout;

    private record ArrayPointerRef(int RelativeOffset, int StringFieldOffset);
    private Dictionary<byte[], List<ArrayPointerRef>> _byteArrayTable = new(new ByteArrayComparer());

    private long _lastRowDataStartOffset;
    private long _lastDataEndOffset;

    public NexDataFileBuilder(NexTableType tableType, NexTableCategory category, NexTableColumnLayout columnLayout, bool usesBaseRow = false, uint baseRowId = 0, 
        ILoggerFactory loggerFactory = null)
    {
        ArgumentNullException.ThrowIfNull(columnLayout);

        if (!Enum.IsDefined(tableType))
            throw new ArgumentException("Invalid nex table type.");

        if (!Enum.IsDefined(category))
            throw new ArgumentException("Invalid nex category type.");

        Type = tableType;
        Category = category;
        UsesBaseRow = usesBaseRow;
        BaseRowId = baseRowId;

        _columnLayout = columnLayout;

        _loggerFactory = loggerFactory;
        if (_loggerFactory is not null)
            _logger = _loggerFactory.CreateLogger(GetType().ToString());
    }

    public void AddRowSet(uint keyId)
    {
        if (Type == NexTableType.RowSets)
            _rowSets.TryAdd(keyId, []);
        else
            throw new InvalidOperationException("Trying to add a row set to a non row-set table.");
    }

    public void AddRow(uint keyId, uint subId, uint arrayIndex, List<object> cells)
    {
        var row = new NexRowBuild()
        {
            RowId = keyId,
            SubId = subId,
            ArrayIndex = arrayIndex,
            Cells = cells,
        };

        if (Type == NexTableType.RowSets)
        {
            AddRowSet(keyId);
            _rowSets[keyId].Add(row);
        }

        _rows.Add(row);
    }

    public void Write(Stream stream)
    {
        var bs = new BinaryStream(stream, ByteConverter.Little);
        bs.WriteUInt32(NexDataFile.MAGIC);
        bs.WriteUInt32(1);
        bs.WriteByte((byte)Type);
        bs.WriteByte((byte)Category);
        bs.WriteBoolean(UsesBaseRow);
        bs.WriteByte(0);
        bs.WriteUInt32(BaseRowId); // base row id
        bs.Position += 0x10;

        switch (Type)
        {
            case NexTableType.Rows:
                WriteRowTable(bs);
                break;

            case NexTableType.RowSets:
                WriteRowSetTable(bs);
                break;

            case NexTableType.DoubleKeyed:
                throw new NotImplementedException();
            default:
                throw new NotSupportedException();
        }
    }

    private void WriteRowTable(BinaryStream bs)
    {
        bs.WriteUInt32(0x30); // offset to rows
        bs.WriteUInt32((uint)_rows.Count);
        bs.Align(0x10, grow: true);

        long rowInfosOffset = bs.Position;
        _lastRowDataStartOffset = bs.Position + (_rows.Count * 0x08);
        _lastDataEndOffset = _lastRowDataStartOffset;

        for (int i = 0; i < _rows.Count; i++)
        {
            NexRowBuild row = _rows[i];

            _lastRowDataStartOffset = _lastDataEndOffset;
            _lastDataEndOffset = _lastRowDataStartOffset + _columnLayout.TotalInlineSize;

            int thisRowInfoOffset = (int)rowInfosOffset + (i * 0x08);
            bs.Position = thisRowInfoOffset;
            bs.WriteUInt32(row.RowId);
            bs.WriteUInt32((uint)(_lastRowDataStartOffset - thisRowInfoOffset));

            bs.Position = _lastRowDataStartOffset;
            WriteRowData(bs, row);
        }

        WriteByteArrayTable(bs);
    }

    private void WriteRowSetTable(BinaryStream bs)
    {
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

            for (int j = 0; j < set.Value.Count; j++)
            {
                NexRowBuild row = set.Value[j];

                _lastRowDataStartOffset = _lastDataEndOffset;
                _lastDataEndOffset = _lastRowDataStartOffset + _columnLayout.TotalInlineSize;

                int thisRowInfoOffset = (int)thisRowSetInfoOffset + (j * 0x0C);
                bs.Position = thisRowInfoOffset;
                bs.WriteUInt32(row.RowId);
                bs.WriteUInt32(row.ArrayIndex);
                bs.WriteUInt32((uint)(_lastRowDataStartOffset - thisRowInfoOffset));

                bs.Position = _lastRowDataStartOffset;
                WriteRowData(bs, row);
            }

            i++;
        }

        long rowDataInfoOffset = _lastDataEndOffset;
        for (i = 0; i < _rows.Count; i++)
        {
            NexRowBuild row = _rows[i];
            long thisRowInfoOffset = rowDataInfoOffset + (i * 0x0C);
            bs.Position = thisRowInfoOffset;

            bs.WriteUInt32(row.RowId);
            bs.WriteUInt32(row.ArrayIndex);
            bs.WriteInt32((int)(row.RowDataOffset - thisRowInfoOffset)); // Will be negative
        }
        _lastDataEndOffset = bs.Position;
        WriteByteArrayTable(bs);

        long tempLastPos = bs.Position;

        bs.Position = subHeaderOffset + 0x0C;
        bs.WriteUInt32((uint)rowDataInfoOffset);

        bs.Position = tempLastPos;
    }

    private void WriteRowData(BinaryStream bs, NexRowBuild row)
    {
        row.RowDataOffset = (int)bs.Position;
        for (int j = 0; j < row.Cells.Count; j++)
            WriteCell(bs, (int)_lastRowDataStartOffset, row.Cells[j], _columnLayout.Columns[j]);

        for (int j = 0; j < row.Cells.Count; j++)
        {
            object obj = row.Cells[j];
            if (obj is not Array)
                continue;

            WriteArray(bs, (int)_lastRowDataStartOffset, (int)_lastRowDataStartOffset + (int)_columnLayout.Columns[j].Offset, obj, _columnLayout.Columns[j]);
        }

        bs.Align(0x04, grow: true);
    }

    private void WriteCell(BinaryStream bs, int rowDataOffset, object cellValue, NexStructColumn column)
    {
        switch (column.Type)
        {
            case NexColumnType.Byte:
                bs.WriteByte((byte)cellValue);
                break;
            case NexColumnType.Short:
                bs.WriteInt16((short)cellValue); 
                break;
            case NexColumnType.Int:
                bs.WriteInt32((int)cellValue);
                break;
            case NexColumnType.UInt:
                bs.WriteUInt32((uint)cellValue);
                break;
            case NexColumnType.Float:
                bs.WriteSingle((float)cellValue);
                break;

            case NexColumnType.ByteArray: // byte arrays are combined with strings at the end of the file
                {
                    byte[] array = (byte[])cellValue;
                    AddByteArray(bs, rowDataOffset, array);
                    bs.Position += 8; // Skip (write later)
                }
                break;

            case NexColumnType.String:
                int relOffset = column.UsesRelativeOffset ? (int)bs.Position + column.RelativeOffsetShift : rowDataOffset;
                AddString(bs, relOffset, cellValue);
                bs.Position += 4; // Skip (write later)
                break;

            case NexColumnType.IntArray:
            case NexColumnType.FloatArray:
            case NexColumnType.StringArray:
            case NexColumnType.CustomStructArray:
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
            case NexColumnType.FloatArray:
                {
                    float[] array = (float[])cellValue;
                    for (int i = 0; i < array.Length; i++)
                        bs.WriteSingle(array[i]);
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
                        List<NexStructColumn> structColumns = _columnLayout.CustomStructDefinitions[column.StructTypeName];
                        for (int j = 0; j < structFields.Length; j++)
                        {
                            WriteCell(bs, arrayOffset, structFields[j], structColumns[j]);
                        }
                    }

                    // Sub-Arrays are always after
                    for (int i = 0; i < array.Length; i++)
                    {
                        object[] structFields = (object[])array[i];
                        List<NexStructColumn> structColumns = _columnLayout.CustomStructDefinitions[column.StructTypeName];
                        for (int j = 0; j < structFields.Length; j++)
                        {
                            object obj = structFields[j];
                            if (obj is not Array)
                                continue;

                            WriteArray(bs, arrayOffsets[i], arrayOffsets[i] + (int)structColumns[j].Offset, obj, structColumns[j]);
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


    internal class NexRowBuild
    {
        public uint RowId { get; set; }
        public uint SubId { get; set; }
        public uint ArrayIndex { get; set; }
        public List<object> Cells { get; set; }
        public int RowDataOffset { get; set; }
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
}
