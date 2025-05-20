using FF16Tools.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Model;

public class ModelBuffer
{
    public Memory<byte> Data;

    public Memory<byte> GetDecompressedData(uint decompressedSize)
    {
        if (Data.Length != decompressedSize)
        {
            byte[] decompressed = new byte[decompressedSize];
            GDeflate.Decompress(Data.Span, decompressed);
            return decompressed;
        }
        else
            return Data;
    }

    public void CompressedData(byte[] decompressedData)
    {
        if (decompressedData.Length == 0)
            throw new Exception($"Buffer given is empty!");

        long sizeCompressed = GDeflate.CompressionSize((uint)decompressedData.Length);

        byte[] compressedBuffer = new byte[sizeCompressed];
        ulong compressedSize = GDeflate.Compress(decompressedData, compressedBuffer);
        Data = compressedBuffer.AsMemory(0, (int)compressedSize);
    }
}
