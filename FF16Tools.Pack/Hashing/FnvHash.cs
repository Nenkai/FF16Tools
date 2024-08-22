using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Hashing;

public struct Fnv1Hash
{
    /// <summary>
    /// The starting point of the FNV hash.
    /// </summary>
    public const uint Offset = 0x811c9dc5;

    /// <summary>
    /// The prime number used to compute the FNV hash.
    /// </summary>
    private const int Prime = 16777619;

    /// <summary>
    /// Gets the current result of the hash function.
    /// </summary>
    public uint HashCode { get; private set; }

    /// <summary>
    /// Creates a new FNV hash initialized to <see cref="Offset"/>.
    /// </summary>
    public static Fnv1Hash Create()
    {
        var result = new Fnv1Hash();
        result.HashCode = Offset;
        return result;
    }

    /// <summary>
    /// Adds the specified byte to the hash.
    /// </summary>
    /// <param name="data">The byte to hash.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Combine(byte data)
    {
        unchecked
        {
            HashCode *= Prime;
            HashCode ^= data;
        }
    }

    /// <summary>
    /// Adds the specified integer to this hash, in little-endian order.
    /// </summary>
    /// <param name="data">The integer to hash.</param>
    public void Combine(Span<byte> data)
    {
        for (int i = 0; i < data.Length; i++)
            Combine(data[i]);
    }

    /// <summary>
    /// Adds the specified integer to this hash, in little-endian order.
    /// </summary>
    /// <param name="data">The integer to hash.</param>
    public void Combine(string data)
    {
        for (int i = 0; i < data.Length; i++)
            Combine((byte)data[i]);
    }

    public static uint HashPath(string str)
    {
        uint result = 0x811c9dc5;
        uint prime = 16777619;
        for (var i = 0; i < str.Length; i++)
        {
            result *= prime;
            result ^= str[i];
        }

        return result;
    }
}

