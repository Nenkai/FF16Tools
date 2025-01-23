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
    public const ulong XOR_KEY = 0x49D18FC870F3824E;

    public static void CryptHeaderPart(Span<byte> data)
    {
        Span<byte> cur = data;
        while (cur.Length >= 8)
        {
            MemoryMarshal.Cast<byte, ulong>(cur)[0] ^= XOR_KEY;
            cur = cur[8..];
        }

        if (cur.Length >= 4)
        {
            MemoryMarshal.Cast<byte, uint>(cur)[0] ^= (uint)(XOR_KEY & 0xFFFFFFFF);
            cur = cur[4..];
        }

        if (cur.Length >= 2)
        {
            MemoryMarshal.Cast<byte, ushort>(cur)[0] ^= (ushort)(XOR_KEY & 0xFFFF);
            cur = cur[2..];
        }

        if (cur.Length >= 1)
            cur[0] ^= (byte)(XOR_KEY & 0xFF);
    }
}
