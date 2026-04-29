
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vortice.DirectStorage;
using SharpGen.Runtime;

namespace FF16Tools.Shared;

public class GDeflate
{
    private static IDStorageCompressionCodec _codec;
    static GDeflate()
    {
        _codec = DirectStorage.DStorageCreateCompressionCodec(CompressionFormat.GDeflate, (uint)Environment.ProcessorCount);
    }

    public static ulong Decompress(Span<byte> compressedData, Span<byte> decompressedData, bool checkSize = true)
    {
        unsafe
        {
            fixed (byte* compPtr = compressedData)
            fixed (byte* decompPtr = decompressedData)
            {
                nuint outUncompressedDataSize = 0;
                _codec.DecompressBuffer((nint)compPtr, (uint)compressedData.Length, (nint)decompPtr, (uint)decompressedData.Length, (nuint)(&outUncompressedDataSize));
                if (checkSize && outUncompressedDataSize != (ulong)decompressedData.Length)
                    throw new InvalidDataException($"Failed to decompress. Expected to decompress 0x{decompressedData.Length:X}, decompressed 0x{outUncompressedDataSize:X}");

                return outUncompressedDataSize;
            }
        }
    }

    public static uint Compress(Span<byte> decompressedData, Span<byte> outputCompressedData)
    {
        unsafe
        {
            fixed (byte* inputDecompChunkPtr = decompressedData)
            fixed (byte* outputCompChunkPtr = outputCompressedData)
            {
                CompressBuffer(_codec, (nint)inputDecompChunkPtr, decompressedData.Length, Compression.BestRatio, (nint)outputCompChunkPtr, outputCompressedData.Length, out long compressedDataSize);
                return (uint)compressedDataSize;
            }
        }
    }

    public static long CompressionSize(ulong size)
    {
        return (long)_codec.CompressBufferBound((nuint)size);
    }

    // This is a hack. CompressBuffer would offer no way to grab back the compressed size
    private static unsafe void CompressBuffer(IDStorageCompressionCodec codec,
        nint uncompressedData, PointerSize uncompressedDataSize, Compression compressionSetting, nint compressedBuffer, PointerSize compressedBufferSize, out long compressedDataSize)
    {
        long value;
        long** vtbl = (long**)codec.NativePointer;
        ((Result)((delegate* unmanaged[Stdcall]<nint, void*, void*, int, void*, void*, void*, int>)(*vtbl)[3])(
            codec.NativePointer, (void*)uncompressedData, uncompressedDataSize, (int)compressionSetting, (void*)compressedBuffer, compressedBufferSize, &value)).CheckError();
        compressedDataSize = value;
    }
}
