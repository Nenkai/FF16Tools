using Syroot.BinaryData;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files;

/// <summary>
/// BinaryStream with some more goodies (mainly pointer utilities). :)
/// </summary>
public class SmartBinaryStream : BinaryStream
{
    public SmartBinaryStream(
        Stream baseStream,
        ByteConverter? byteConverter = null,
        Encoding? encoding = null,
        BooleanCoding booleanCoding = BooleanCoding.Byte,
        DateTimeCoding dateTimeCoding = DateTimeCoding.NetTicks,
        StringCoding stringCoding = StringCoding.VariableByteCount,
        bool leaveOpen = false)
        : base(baseStream, byteConverter, encoding, booleanCoding, dateTimeCoding, stringCoding, leaveOpen)
    {
        
    }

    /// <summary>
    /// Offset storage for written objects. Object -> Offset
    /// </summary>
    public Dictionary<object, long> ObjectOffsetStorage = [];

    /// <summary>
    /// List of string pointers.
    /// </summary>
    public List<StringPointer> StringPointers { get; private set; } = [];

    /// <summary>
    /// Reads a string pointer.
    /// </summary>
    /// <param name="relativeBaseOffset">Relative position for the offset.</param>
    /// <param name="stringCoding"></param>
    /// <returns></returns>
    public string ReadStringPointer(long relativeBaseOffset = 0, StringCoding stringCoding = StringCoding.ZeroTerminated)
    {
        int strOffset = ReadInt32();

        long tempPos = Position;

        Position = relativeBaseOffset + strOffset;
        string str = this.ReadString(stringCoding);

        Position = tempPos;
        return str;
    }

    /// <summary>
    /// Reads a struct.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T ReadStruct<T>() where T : ISerializableStruct, new()
    {
        var @struct = new T();
        @struct.Read(this);
        return @struct;
    }

    /// <summary>
    /// Reads a struct pointer.
    /// </summary>
    /// <param name="relativeBaseOffset">Relative position for the offset.</param>
    /// <returns></returns>
    public T ReadStructPointer<T>(long relativeBaseOffset = 0) where T : ISerializableStruct, new()
    {
        int structOffset = ReadInt32();

        long tempPos = Position;

        var @struct = new T();
        Position = relativeBaseOffset + structOffset;
        @struct.Read(this);

        Position = tempPos;
        return @struct;
    }

    /// <summary>
    /// Adds an object to the offset storage. This is used to remember where an object was written.
    /// </summary>
    /// <param name="obj"></param>
    public void AddObjectPointer(object obj)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        ObjectOffsetStorage.Add(obj, Position);
    }


    /// <summary>
    /// Gets the offset of an object that was previously written, and registered with <see cref="AddObjectPointer(object)"/>.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public long GetObjectPointerOffset(object obj)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        return ObjectOffsetStorage[obj];
    }

    /// <summary>
    /// Writes a pointer/offset to a certain object. <br/> 
    /// Object must have been previously written, and registered with <see cref="AddObjectPointer(object)"/>.
    /// </summary>
    /// <param name="obj">Target object.</param>
    /// <param name="relativeBaseOffset">Base offset.</param>
    /// <param name="writeZeroIfNullOrNotFound">Whether to write zero as offset rather than throw if the object is null, or not found in the storage.</param>
    /// <exception cref="Exception"></exception>
    public void WriteObjectPointer(object? obj, long relativeBaseOffset, bool writeZeroIfNullOrNotFound = false)
    {
        if (obj is not null && ObjectOffsetStorage.TryGetValue(obj, out long absoluteObjOffset))
        {
            long relativeObjectOffset = absoluteObjOffset - relativeBaseOffset;
            WriteInt32((int)relativeObjectOffset); // May be reverse, which is intended
        }
        else
        {
            if (writeZeroIfNullOrNotFound)
                WriteInt32(0);
            else
                throw new Exception($"Object {obj} not found in offset storage.");
        }
    }

    /// <summary>
    /// Adds a string to the list of string pointers and advances by 1 int.<br/>
    /// This is used to write a compact string table later on with deduped strings, using <see cref="WriteStringTable"/>.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="relativeBaseOffset"></param>
    public void AddStringPointer(string? str, long relativeBaseOffset)
    {
        StringPointers.Add(new StringPointer(str, Position, relativeBaseOffset));
        WriteInt32(0); // Will be written on finalization
    }

    /// <summary>
    /// Registers a string list to be written using an offset table for each string at the specified data offset, and its offset+count at the current position. Advances by two ints.<br/>
    /// This is used to write a compact string table later on with deduped strings, using <see cref="WriteStringTable"/>.
    /// </summary>
    /// <typeparam name="TStruct"></typeparam>
    /// <param name="basePos"></param>
    /// <param name="structArray"></param>
    public void AddStringPointers(long basePos, IList<string> strs, ref long dataOffset)
    {
        long arrayOffsetOffset = Position;

        long offsetTableOffset = dataOffset;
        Position = offsetTableOffset;

        for (int i = 0; i < strs.Count; i++)
            StringPointers.Add(new StringPointer(strs[i], Position + (i * 0x04), offsetTableOffset));

        Position = arrayOffsetOffset;
        WriteInt32((int)(dataOffset - basePos));
        WriteUInt32((uint)strs.Count);

        dataOffset += strs.Count * sizeof(int);
    }

    /// <summary>
    /// Writes the string table. Make sure to have set the <see cref="StringCoding"/> property in the constructor to avoid any surprises.
    /// </summary>
    public void WriteStringTable(bool writeEmptyFirst = true)
    {
        long lastDataPos = Position;

        Dictionary<string, long> _writtenStrings = [];
        // An empty string always goes first (if present.)
        if (writeEmptyFirst && StringPointers.Any(e => e.String == string.Empty))
        {
            _writtenStrings.Add(string.Empty, Position);
            this.WriteString(string.Empty, StringCoding);

            lastDataPos = Position;
        }

        foreach (var stringRef in StringPointers)
        {
            if (!_writtenStrings.TryGetValue(stringRef.String!, out long strOffset))
            {
                strOffset = Position;
                this.WriteString(stringRef.String, StringCoding);
                _writtenStrings.Add(stringRef.String!, strOffset);

                lastDataPos = Position;
                Position = stringRef.PointerOffset;
            }

            Position = stringRef.PointerOffset;
            this.WriteInt32((int)(strOffset - stringRef.RelativeBaseOffset));

            Position = lastDataPos;
        }
    }

    /// <summary>
    /// Reads a null terminated string out of a fixed-size string buffer.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public string ReadStringFix(uint bufferSize)
    {
        Span<byte> buffer = bufferSize <= 1024 ? stackalloc byte[(int)bufferSize] : new byte[bufferSize];
        ReadExactly(buffer);

        int end = buffer.IndexOf<byte>(0);
        if (end == -1)
            throw new InvalidDataException($"ReadStringFix: No null terminator within string buffer with size {bufferSize}.");

        return this.Encoding.GetString(buffer[..end]);
    }

    /// <summary>
    /// Writes a fixed-size string buffer containing a null-terminated string.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="size"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void WriteStringFix(string str, uint bufferSize)
    {
        int byteCount = Encoding.GetByteCount(str);
        if (byteCount > bufferSize - 1)
            throw new ArgumentOutOfRangeException($"WriteStringFix: String is too large to fit in buffer. (string byte count: {byteCount}, bufferSize: {bufferSize})");

        Span<byte> buffer = bufferSize <= 1024 ? stackalloc byte[(int)bufferSize] : new byte[bufferSize];
        this.Encoding.GetBytes(str, buffer);
        buffer[byteCount] = 0;

        Write(buffer);
    }

    /// <summary>
    /// Writes padding. This is prefered over adjusting Position or using Seek(), as seeking does not ensure bytes to be written by default.
    /// </summary>
    /// <param name="paddingSize"></param>
    public void WritePadding(int paddingSize)
    {
        for (int i = 0; i < paddingSize; i++)
            WriteByte(0);
    }

    /// <summary>
    /// Reads and checks padding. If the padding contains any non-zero bytes, this method will throw.
    /// </summary>
    /// <param name="paddingSize"></param>
    /// <exception cref="InvalidDataException"></exception>
    public void ReadCheckPadding(int paddingSize)
    {
        Span<byte> buffer = paddingSize <= 1024 ? stackalloc byte[(int)paddingSize] : new byte[paddingSize];
        ReadExactly(buffer);

        if (buffer.ContainsAnyExcept<byte>(0))
            throw new InvalidDataException("SkipCheckPadding failed. Padding had non-zero data.");
    }

    public TStruct ReadStructMarshal<TStruct>() where TStruct : unmanaged
    {
        int size = Unsafe.SizeOf<TStruct>();
        Span<byte> buffer = size > 1024 ? new byte[size] : stackalloc byte[size];

        int bytesRead = Read(buffer);
        if (bytesRead != size)
            throw new EndOfStreamException();

        return MemoryMarshal.Read<TStruct>(buffer);
    }

    /// <summary>
    /// Writes an array of inline structs starting from the current offset.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="elementCount"></param>
    /// <returns></returns>
    public void WriteStructArray<T>(IEnumerable<T> list) where T : ISerializableStruct, new()
    {
        foreach (T elem in list)
        {
            long startPos = Position;
            elem.Write(this);
            if (Position != startPos + elem.GetSize())
                throw new InvalidDataException("WriteArrayOfStructs: Struct serialization size mismatch. Make sure Write and GetSize() matches.");
        }
    }

    /// <summary>
    /// Writes any struct, using marshaling.
    /// </summary>
    /// <typeparam name="TStruct"></typeparam>
    /// <param name="struct"></param>
    public void WriteStructMarshal<TStruct>(TStruct @struct) where TStruct : struct
    {
        Span<byte> buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref @struct, 1));
        Write(buffer);
    }


    /// <summary>
    /// Writes a struct at the specified data position, and its offset at the current position. Advances by one int. <br/><br/>
    /// NOTE: Ending position is expected to be at, or after the struct data offset (incase of nested structs that appears after this one being serialized). <br/>
    /// Make sure to adjust your function to point to the end of the function on return.
    /// </summary>
    /// <typeparam name="TStruct"></typeparam>
    /// <param name="basePos"></param>
    /// <param name="struct"></param>
    public void WriteStructPointer<TStruct>(long basePos, TStruct @struct, ref long lastDataPos) where TStruct : ISerializableStruct
    {
        long structOffsetOffset = Position;
        long structEndOffset = lastDataPos + @struct.GetSize();

        Position = lastDataPos;
        @struct.Write(this);
        if (Position > structEndOffset)
            structEndOffset = Position;

        Position = structOffsetOffset;
        WriteInt32((int)(lastDataPos - basePos));

        lastDataPos = structEndOffset;
    }

    /// <summary>
    /// Writes a struct array using an offset table for each element at the specified data offset, and its offset+count at the current position. Advances by two ints.
    /// </summary>
    /// <typeparam name="TStruct"></typeparam>
    /// <param name="basePos"></param>
    /// <param name="structArray"></param>
    public void WriteStructArrayPointerWithOffsetTable32<TStruct>(long basePos, IList<TStruct> structArray, ref long dataOffset) where TStruct : ISerializableStruct
    {
        long arrayOffsetOffset = Position;
        long tablePos = dataOffset;
        long lastDataPos = tablePos + (structArray.Count * sizeof(int));

        for (int i = 0; i < structArray.Count; i++)
        {
            Position = tablePos + (i * sizeof(int));
            WriteStructPointer(tablePos, structArray[i], ref lastDataPos);
            if (Position > lastDataPos)
                lastDataPos = Position; // Incase the nested structure wrote further data.
        }

        Position = arrayOffsetOffset;
        WriteInt32((int)(dataOffset - basePos));
        WriteUInt32((uint)structArray.Count);

        dataOffset = lastDataPos;
    }

    /// <summary>
    /// Writes a struct array at the specified data offset, and its offset+count at the current position. Advances by two ints. <br/>
    /// Creates a <see cref="StructMarker"/> with the last data position. Use <see cref="GetMarker"/> to retrieve it.
    /// </summary>
    /// <typeparam name="TStruct"></typeparam>
    /// <param name="basePos"></param>
    /// <param name="structArray"></param>
    public void WriteStructArrayPointer<TStruct>(long basePos, IList<TStruct> structArray, ref long dataOffset) where TStruct : ISerializableStruct, new()
    {
        long arrayOffsetOffset = Position;
        uint structSize = structArray.Count != 0 ? structArray[0].GetSize() : 0;
        uint inlineArraySize = (uint)(structSize * structArray.Count);
        long lastDataPos = dataOffset + inlineArraySize;

        for (int i = 0; i < structArray.Count; i++)
        {
            Position = dataOffset + (i * structSize);
            using (PushMarker(lastDataPos))
            {
                structArray[i].Write(this);
                if (Position > lastDataPos)
                    lastDataPos = Position; // Incase the nested structure wrote further data.
            }
        }

        Position = arrayOffsetOffset;
        WriteInt32((int)(dataOffset - basePos));
        WriteUInt32((uint)structArray.Count);

        dataOffset = lastDataPos;
    }

    /// <summary>
    /// Reads an array of inline structs starting from the current offset.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="elementCount"></param>
    /// <returns></returns>
    public List<T> ReadStructArray<T>(uint elementCount) where T : ISerializableStruct, new()
    {
        List<T> elements = new List<T>((int)elementCount);

        long basePos = Position;
        for (int i = 0; i < elementCount; i++)
        {
            T elem = new();
            Position = basePos + i * elem.GetSize();
            elem.Read(this);
            elements.Add(elem);
        }

        return elements;
    }

    /// <summary>
    /// Reads an array of inline structs starting from the specified offset.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="startOffset"></param>
    /// <param name="elementCount"></param>
    /// <returns></returns>
    public List<T> ReadStructArray<T>(long startOffset, uint elementCount) where T : ISerializableStruct, new()
    {
        List<T> elements = new List<T>((int)elementCount);

        for (int i = 0; i < elementCount; i++)
        {
            T elem = new();
            Position = startOffset + i * elem.GetSize();
            elem.Read(this);
            elements.Add(elem);
        }

        return elements;
    }

    /// <summary>
    /// Reads an array of structs, at the incoming offset/pointer (int) + count (int) pair.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="startOffset"></param>
    /// <returns></returns>
    public List<T> ReadStructArrayFromOffsetCount<T>(long startOffset) where T : ISerializableStruct, new()
    {
        int offset = ReadInt32();
        uint count = ReadUInt32();
        long tmpPos = Position;

        List<T> elements = new List<T>((int)count);

        for (int i = 0; i < count; i++)
        {
            T elem = new();
            Position = startOffset + offset + (i * elem.GetSize());
            elem.Read(this);
            elements.Add(elem);
        }

        Position = tmpPos;
        return elements;
    }

    /// <summary>
    /// Reads an array of structs, at the incoming offset/pointer (int) + count (int) pair, and passes the element to the specified callback. <br/>
    /// Useful to store it in a different collection than the list returned by <see cref="ReadStructArrayFromOffsetCount{T}(long)"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="startOffset"></param>
    /// <returns></returns>
    public void ReadStructArrayFromOffsetCountWithCallback<T>(long startOffset, Action<T> callback) where T : ISerializableStruct, new()
    {
        int offset = ReadInt32();
        uint count = ReadUInt32();
        long tmpPos = Position;

        List<T> elements = new List<T>((int)count);

        for (int i = 0; i < count; i++)
        {
            T elem = new();
            Position = startOffset + offset + (i * elem.GetSize());
            elem.Read(this);
            elements.Add(elem);

            callback(elem);
        }

        Position = tmpPos;
    }

    /// <summary>
    /// Reads an array of structs, at the incoming offset/pointer (int) + count (int) pair which points to an offset for each entry.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="startOffset"></param>
    /// <returns></returns>
    public List<T> ReadStructArrayFromOffsetCountToOffsetTable32<T>(long startOffset) where T : ISerializableStruct, new()
    {
        int offset = ReadInt32();
        uint count = ReadUInt32();
        long tmpPos = Position;

        List<T> elements = new List<T>((int)count);

        for (int i = 0; i < count; i++)
        {
            Position = startOffset + offset + (i * 4);
            uint dataOffset = ReadUInt32();

            Position = startOffset + offset + dataOffset;
            T elem = new T();
            elem.Read(this);
            elements.Add(elem);
        }

        Position = tmpPos;
        return elements;
    }

    /// <summary>
    /// Reads strings starting from the specified offset, using the offset table for each entry at the specified offset.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="startOffset"></param>
    /// <param name="elementCount"></param>
    /// <returns></returns>
    public List<string> ReadStringsFromOffsetTable32(long startOffset, uint elementCount, StringCoding stringCoding = StringCoding.ZeroTerminated)
    {
        List<string> elements = new List<string>((int)elementCount);

        for (int i = 0; i < elementCount; i++)
        {
            Position = startOffset + (i * 4);
            uint dataOffset = ReadUInt32();

            Position = startOffset + dataOffset;
            string str = this.ReadString(stringCoding);
            elements.Add(str);
        }

        return elements;
    }

    /// <summary>
    /// Reads structs starting from the specified offset, using the offset table for each entry at the specified offset.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="startOffset"></param>
    /// <param name="elementCount"></param>
    /// <returns></returns>
    public List<T> ReadStructsFromOffsetTable32<T>(long startOffset, uint elementCount) where T : ISerializableStruct, new()
    {
        List<T> elements = new List<T>((int)elementCount);

        for (int i = 0; i < elementCount; i++)
        {
            T elem = new();
            Position = startOffset + (i * 4);
            uint dataOffset = ReadUInt32();

            Position = startOffset + dataOffset;
            elem.Read(this);
            elements.Add(elem);
        }

        return elements;
    }

    public void WriteUint32AtOffset(long target, long relativePosition = 0)
    {
        long pos = Position;
        using (TemporarySeek(target, SeekOrigin.Begin))
        {
            Write((uint)(pos - relativePosition));
        }
    }

    private Stack<StructMarker> _markers = [];

    /// <summary>
    /// Creates a marker. Mainly used to get the current ending data position for the current struct. Use this with the <see href="using"/> keyword.
    /// </summary>
    /// <param name="endPos"></param>
    /// <returns></returns>
    public StructMarker PushMarker(long endPos)
    {
        var t = new StructMarker(this, Position, endPos);

        _markers.Push(t);
        return t;
    }

    public StructMarker GetMarker()
    {
        return _markers.Peek();
    }

    public void PopMarker()
    {
        _markers.Pop();
    }

    public record StringPointer(string? String, long PointerOffset, long RelativeBaseOffset);
}

public class StructMarker : IDisposable
{
    public long BasePos { get; }
    public long LastDataPosition { get; set; }
    public SmartBinaryStream Stream { get; }

    public StructMarker(SmartBinaryStream stream, long basePos, long endPos)
    {
        Stream = stream;
        BasePos = basePos;
        LastDataPosition = endPos;
    }

    public void Dispose()
    {
        Stream.PopMarker();
    }
}