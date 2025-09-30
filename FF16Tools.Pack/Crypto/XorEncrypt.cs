using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Pack.Crypto;

public class XorEncrypt
{
    public static void CryptHeaderPart(Span<byte> data, ulong key)
    {
        Span<byte> cur = data;
        while (cur.Length >= 8)
        {
            MemoryMarshal.Cast<byte, ulong>(cur)[0] ^= key;
            cur = cur[8..];
        }

        if (cur.Length >= 4)
        {
            MemoryMarshal.Cast<byte, uint>(cur)[0] ^= (uint)(key & 0xFFFFFFFF);
            cur = cur[4..];
        }

        if (cur.Length >= 2)
        {
            MemoryMarshal.Cast<byte, ushort>(cur)[0] ^= (ushort)(key & 0xFFFF);
            cur = cur[2..];
        }

        if (cur.Length >= 1)
            cur[0] ^= (byte)(key & 0xFF);
    }
}
