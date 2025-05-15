using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
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
        ByteConverter byteConverter = null,
        Encoding encoding = null,
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
    /// Adds a string to the list of string pointers. <br/>
    /// This is used to write a compact string table later on with deduped strings.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="relativeBaseOffset"></param>
    public void AddStringPointer(string? str, long relativeBaseOffset)
    {
        StringPointers.Add(new StringPointer(str, Position, relativeBaseOffset));
        WriteInt32(0); // Will be written on finalization
    }

    /// <summary>
    /// Writes the string table.
    /// </summary>
    public void WriteStringTable()
    {
        long lastDataPos = Position;

        Dictionary<string, long> _writtenStrings = [];
        // An empty string always goes first (if present.)
        if (StringPointers.Any(e => e.String == string.Empty))
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
    /// Writes padding. This is prefered over adjusting Position or using Seek(), as seeking does not ensure bytes to be written by default.
    /// </summary>
    /// <param name="paddingSize"></param>
    public void WritePadding(int paddingSize)
    {
        for (int i = 0; i < paddingSize; i++)
            WriteByte(0);
    }

    public List<T> ReadArrayOfStructs<T>(long startOffset, int elementCount) where T : ISerializableStruct, new()
    {
        List<T> elements = [];

        for (int i = 0; i < elementCount; i++)
        {
            T elem = new();
            Position = startOffset + i * elem.GetSize();
            elem.Read(this);
            elements.Add(elem);
        }

        return elements;
    }

    public record StringPointer(string? String, long PointerOffset, long RelativeBaseOffset);
}
