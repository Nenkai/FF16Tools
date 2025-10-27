using Syroot.BinaryData;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
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
    /// Writes the string table. Make sure to have set the <see cref="StringCoding"/> property in the constructor to avoid any surprises.
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

    public void WriteVector2(Vector2 vec)
    {
        WriteSingle(vec.X);
        WriteSingle(vec.Y);
    }

    public void WriteVector3(Vector3 vec)
    {
        WriteSingle(vec.X);
        WriteSingle(vec.Y);
        WriteSingle(vec.Z);
    }

    public void WriteVector4(Vector4 vec)
    {
        WriteSingle(vec.X);
        WriteSingle(vec.Y);
        WriteSingle(vec.Z);
        WriteSingle(vec.Z);
    }

    public Vector2 ReadVector2()
    {
        return new Vector2(
            ReadSingle(),
            ReadSingle()
        );
    }
    
    public Vector3 ReadVector3()
    {
        return new Vector3(
            ReadSingle(),
            ReadSingle(),
            ReadSingle()
        );
    }

    public Vector4 ReadVector4()
    {
        return new Vector4(
            ReadSingle(),
            ReadSingle(),
            ReadSingle(),
            ReadSingle()
        );
    }

    public Point ReadPoint()
    {
        return new Point(
            ReadInt32(),
            ReadInt32()
        );
    }

    /// <summary>
    /// Writes an array of inline structs starting from the current offset.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="elementCount"></param>
    /// <returns></returns>
    public void WriteArrayOfStructs<T>(IEnumerable<T> list) where T : ISerializableStruct, new()
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
    /// Reads an array of inline structs starting from the current offset.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="elementCount"></param>
    /// <returns></returns>
    public List<T> ReadArrayOfStructs<T>(uint elementCount) where T : ISerializableStruct, new()
    {
        List<T> elements = [];

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
    public List<T> ReadArrayOfStructs<T>(long startOffset, uint elementCount) where T : ISerializableStruct, new()
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

    public record StringPointer(string? String, long PointerOffset, long RelativeBaseOffset);
}
