using SharpGen.Runtime;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortice.Direct3D;
using Vortice.Direct3D12;
using Vortice.Direct3D12.Debug;
using Vortice.DirectStorage;
using Vortice.Mathematics;
using Vortice.Win32;

using static Vortice.Direct3D12.D3D12;
using static Vortice.DirectStorage.DirectStorage;

namespace FF16Tools.Files.Textures;

public class TextureDecompressEmulationTest
{
    private static void DebugCallback(MessageCategory category, MessageSeverity severity, MessageId id, string description)
    {
        System.Diagnostics.Debug.WriteLine($"[{category}] [{severity}] {description}");
    }

    public static void Test(string fileName, TextureFile textureFile)
    {
        if (D3D12GetDebugInterface(out ID3D12Debug? debugInterface).Success)
        {
            debugInterface?.EnableDebugLayer();
            debugInterface?.Dispose();
        }

        using ID3D12Device device = D3D12CreateDevice<ID3D12Device>(null, FeatureLevel.Level_12_1);

        var infoQueue1 = device.QueryInterfaceOrNull<ID3D12InfoQueue1>();
        if (infoQueue1 != null)
        {
            infoQueue1.RegisterMessageCallback(DebugCallback, MessageCallbackFlags.None);
        }

        using IDStorageFactory factory = DStorageGetFactory<IDStorageFactory>();

        Result result = factory.OpenFile(fileName, out IDStorageFile? file);
        if (result.Failure)
        {
            Console.WriteLine($"The file '{fileName}' could not be opened. HRESULT={result}");
            //ShowHelpText();
            return;
        }

        // Create a DirectStorage queue which will be used to load data into a buffer on the GPU.
        QueueDesc queueDesc = new QueueDesc
        {
            Capacity = MaxQueueCapacity,
            Priority = Priority.Normal,
            SourceType = RequestSourceType.File,
            Device = device
        };

        using IDStorageQueue queue = factory.CreateQueue(queueDesc);

        // Create the ID3D12Resource buffer which will be populated with the file's contents
        HeapProperties bufferHeapProps = new HeapProperties(HeapType.Custom, CpuPageProperty.WriteCombine, MemoryPool.L0);
        ResourceDescription bufferDesc = ResourceDescription.Texture2D((Vortice.DXGI.Format)TextureUtils.TexPixelFormatToDxgiFormat(textureFile.Textures[0].PixelFormat), textureFile.Textures[0].Width, textureFile.Textures[0].Height);

        using ID3D12Resource bufferResource = device.CreateCommittedResource(
            bufferHeapProps,
            HeapFlags.None,
            bufferDesc,
            ResourceStates.Common
        );

        var chunk = textureFile.TextureChunks[textureFile.Textures[0].ChunkIndex];

        // Enqueue a request to read the file contents into a destination D3D12 buffer resource.
        // Note: The example request below is performing a single read of the entire file contents.
        Request request = new Request();
        request.Options.SourceType = RequestSourceType.File;
        request.Source.File.Source = file;
        request.Source.File.Offset = chunk.CompressedDataOffset;
        request.Source.File.Size = chunk.CompressedChunkSize;

        if (chunk.CompressedChunkSize != chunk.DecompressedChunkSize)
            request.Options.CompressionFormat = CompressionFormat.GDeflate;

        // Game does this, i think
        if (false /*chunk.TypeBits == 1*/)
        {
            request.Options.DestinationType = RequestDestinationType.MultipleSubresources;
            request.Destination.MultipleSubresources.Resource = bufferResource;
            request.Destination.MultipleSubresources.FirstSubresource = 0;
        }
        else
        {
            request.Options.DestinationType = RequestDestinationType.TextureRegion;
            request.Destination.Texture.Resource = bufferResource;
            request.Destination.Texture.SubresourceIndex = 0;
            request.Destination.Texture.Region = new Box(0, 0, 0, textureFile.Textures[0].Width, textureFile.Textures[0].Height, 1);
        }

        request.UncompressedSize = chunk.DecompressedChunkSize;
        queue.EnqueueRequest(request);

        // Configure a fence to be signaled when the request is completed
        using ID3D12Fence fence = device.CreateFence();
        using AutoResetEvent fenceEvent = new AutoResetEvent(false);

        ulong fenceValue = 1;
        fence.SetEventOnCompletion(fenceValue, fenceEvent).CheckError();
        queue.EnqueueSignal(fence, fenceValue);

        // Tell DirectStorage to start executing all queued items.
        queue.Submit();

        // Wait for the submitted work to complete
        Console.WriteLine("Waiting for the DirectStorage request to complete...");
        fenceEvent.WaitOne();

        // Check the status array for errors.
        // If an error was detected the first failure record
        // can be retrieved to get more details.
        ErrorRecord errorRecord = queue.RetrieveErrorRecord();
        if (errorRecord.FirstFailure.HResult.Failure)
        {
            //
            // errorRecord.FailureCount - The number of failed requests in the queue since the last
            //                            RetrieveErrorRecord call.
            // errorRecord.FirstFailure - Detailed record about the first failed command in the enqueue order.
            //
            Console.WriteLine($"The DirectStorage request failed! HRESULT={errorRecord.FirstFailure.HResult}");
        }
        else
        {
            ulong[] rowSizes = new ulong[5];
            PlacedSubresourceFootPrint[] footprints = new PlacedSubresourceFootPrint[1];
            device.GetCopyableFootprints(bufferResource.Description, 0, 1, 0ul, footprints, null!, rowSizes, out ulong totalBytes);

            //byte[] outBuf = new byte[totalBytes];
            //var res = bufferResource.ReadFromSubresource(outBuf, footprints[0].Footprint.RowPitch, 1, 0, new Box(0, 0, 0, 926, 330, 1));
        }
    }
}
