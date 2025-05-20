
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

    public static void Decompress(Span<byte> compressedData, Span<byte> decompressedData)
    {
        unsafe
        {
            fixed (byte* buffer = compressedData)
            fixed (byte* buffer2 = decompressedData)
            {
                _codec.DecompressBuffer((nint)buffer, compressedData.Length, (nint)buffer2, decompressedData.Length, decompressedData.Length);
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

    public static long CompressionSize(long size)
    {
        return _codec.CompressBufferBound(size);
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
