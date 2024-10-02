using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Shared.Dds;

public class DxgiUtils
{
    public const int D3D12_TEXTURE_DATA_PITCH_ALIGNMENT = 256;

    public static uint BitsPerPixel(DXGI_FORMAT fmt)
    {
        switch (fmt)
        {
            case DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT:
            case DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_SINT:
                return 128;

            case DXGI_FORMAT.DXGI_FORMAT_R32G32B32_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT:
            case DXGI_FORMAT.DXGI_FORMAT_R32G32B32_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R32G32B32_SINT:
                return 96;

            case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_FLOAT:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_SNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_SINT:
            case DXGI_FORMAT.DXGI_FORMAT_R32G32_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT:
            case DXGI_FORMAT.DXGI_FORMAT_R32G32_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R32G32_SINT:
            case DXGI_FORMAT.DXGI_FORMAT_R32G8X24_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT_S8X24_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_X32_TYPELESS_G8X24_UINT:
                return 64;

            case DXGI_FORMAT.DXGI_FORMAT_R10G10B10A2_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_R10G10B10A2_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R10G10B10A2_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R11G11B10_FLOAT:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_SNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_SINT:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16_FLOAT:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16_SNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16_SINT:
            case DXGI_FORMAT.DXGI_FORMAT_R32_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT:
            case DXGI_FORMAT.DXGI_FORMAT_R32_FLOAT:
            case DXGI_FORMAT.DXGI_FORMAT_R32_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R32_SINT:
            case DXGI_FORMAT.DXGI_FORMAT_R24G8_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_D24_UNORM_S8_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R24_UNORM_X8_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_X24_TYPELESS_G8_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R9G9B9E5_SHAREDEXP:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8_B8G8_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_G8R8_G8B8_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_UNORM_SRGB:
                return 32;

            case DXGI_FORMAT.DXGI_FORMAT_R8G8_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8_SNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8_SINT:
            case DXGI_FORMAT.DXGI_FORMAT_R16_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_R16_FLOAT:
            case DXGI_FORMAT.DXGI_FORMAT_D16_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R16_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R16_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R16_SNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R16_SINT:
            case DXGI_FORMAT.DXGI_FORMAT_B5G6R5_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_B5G5R5A1_UNORM:
                return 16;

            case DXGI_FORMAT.DXGI_FORMAT_R8_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_R8_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R8_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R8_SNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R8_SINT:
            case DXGI_FORMAT.DXGI_FORMAT_A8_UNORM:
                return 8;

            case DXGI_FORMAT.DXGI_FORMAT_R1_UNORM:
                return 1;

            case DXGI_FORMAT.DXGI_FORMAT_BC1_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_BC4_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM:
                return 4;

            case DXGI_FORMAT.DXGI_FORMAT_BC2_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_BC3_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_BC5_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC6H_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16:
            case DXGI_FORMAT.DXGI_FORMAT_BC6H_SF16:
            case DXGI_FORMAT.DXGI_FORMAT_BC7_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB:
                return 8;

            default:
                return 0;
        }
    }

    public static bool IsBCnFormat(DXGI_FORMAT fmt)
    {
        switch (fmt)
        {
            case DXGI_FORMAT.DXGI_FORMAT_BC1_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_BC4_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC2_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_BC3_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_BC5_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC6H_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16:
            case DXGI_FORMAT.DXGI_FORMAT_BC6H_SF16:
            case DXGI_FORMAT.DXGI_FORMAT_BC7_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB:
                return true;
        }

        return false;
    }

    private static uint Align(uint x, uint alignment)
    {
        uint mask = ~(alignment - 1);
        return (x + (alignment - 1)) & mask;
    }

    public static int ComputePitch(DXGI_FORMAT fmt, uint width, uint height,
        out ulong rowPitch, out ulong slicePitch, out ulong alignedSlicePitch)
    {
        rowPitch = 0;
        slicePitch = 0;
        alignedSlicePitch = 0;

        ulong pitch, slice, aSlice;
    
        switch (fmt)
        {
            case DXGI_FORMAT.DXGI_FORMAT_BC1_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_BC4_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM:
                {
                    ulong nbw = Math.Max(1u, ((ulong)(width) + 3u) / 4u);
                    ulong nbh = Math.Max(1u, ((ulong)(height) + 3u) / 4u);
                    pitch = nbw * 8u;
                    slice = pitch * nbh;
                    aSlice = Align((uint)pitch, D3D12_TEXTURE_DATA_PITCH_ALIGNMENT) * nbh;
                }
            
                break;
    
            case DXGI_FORMAT.DXGI_FORMAT_BC2_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_BC3_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_BC5_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC6H_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16:
            case DXGI_FORMAT.DXGI_FORMAT_BC6H_SF16:
            case DXGI_FORMAT.DXGI_FORMAT_BC7_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB:
                {
                    ulong nbw = Math.Max(1u, ((ulong)(width) + 3u) / 4u);
                    ulong nbh = Math.Max(1u, ((ulong)(height) + 3u) / 4u);
                    pitch = nbw * 16u;
                    slice = pitch * nbh;
                    aSlice = Align((uint)pitch, D3D12_TEXTURE_DATA_PITCH_ALIGNMENT) * nbh;
                }
                break;
    
            case DXGI_FORMAT.DXGI_FORMAT_R8G8_B8G8_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_G8R8_G8B8_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_YUY2:
                pitch = (((ulong)(width) + 1u) >> 1) * 4u;
                slice = pitch * height;
                aSlice = Align((uint)pitch, D3D12_TEXTURE_DATA_PITCH_ALIGNMENT) * (ulong)(height);
                break;
            
            case DXGI_FORMAT.DXGI_FORMAT_Y210:
            case DXGI_FORMAT.DXGI_FORMAT_Y216:
                pitch = (((ulong)(width) + 1u) >> 1) * 8u;
                slice = pitch * height;
                aSlice = Align((uint)pitch, D3D12_TEXTURE_DATA_PITCH_ALIGNMENT) * (ulong)(height);
                break;
            
            case DXGI_FORMAT.DXGI_FORMAT_NV12:
            case DXGI_FORMAT.DXGI_FORMAT_420_OPAQUE:
                if ((height % 2) != 0)
                {
                    // Requires a height alignment of 2.
                    return -1;
                }

                pitch = (((ulong)(width) + 1u) >> 1) * 2u;
                slice = pitch * (height + (((ulong)(height) + 1u) >> 1));
                aSlice = Align((uint)pitch, D3D12_TEXTURE_DATA_PITCH_ALIGNMENT) * (height + (((ulong)(height) + 1u) >> 1));
                break;
            
            case DXGI_FORMAT.DXGI_FORMAT_P010:
            case DXGI_FORMAT.DXGI_FORMAT_P016:
                if ((height % 2) != 0)
                {
                    // Requires a height alignment of 2.
                    return -1;
                }
    
                pitch = (((ulong)(width) + 1u) >> 1) * 4u;
                slice = pitch * (height + (((ulong)(height) + 1u) >> 1));
                aSlice = Align((uint)pitch, D3D12_TEXTURE_DATA_PITCH_ALIGNMENT) * (height + (((ulong)(height) + 1u) >> 1));
                break;

            case DXGI_FORMAT.DXGI_FORMAT_NV11:
                pitch = (((ulong)(width) + 3u) >> 2) * 4u;
                slice = pitch * (ulong)(height) * 2u;
                aSlice = Align((uint)pitch, D3D12_TEXTURE_DATA_PITCH_ALIGNMENT) * (ulong)(height) * 2u;
                break;
    
            case DXGI_FORMAT.DXGI_FORMAT_P208:
                pitch = (((ulong)(width) + 1u) >> 1) * 2u;
                slice = pitch * (ulong)(height) * 2u;
                aSlice = Align((uint)pitch, D3D12_TEXTURE_DATA_PITCH_ALIGNMENT) * (ulong)(height) * 2u;
                break;
            
            case DXGI_FORMAT.DXGI_FORMAT_V208:
                if ((height % 2) != 0)
                {
                    // Requires a height alignment of 2.
                    return -1;
                }
                pitch = width;
                slice = pitch * (height + ((((ulong)(height) + 1u) >> 1) * 2u));
                aSlice = Align((uint)pitch, D3D12_TEXTURE_DATA_PITCH_ALIGNMENT) * (height + ((((ulong)(height) + 1u) >> 1) * 2u));
                break;
            
            case DXGI_FORMAT.DXGI_FORMAT_V408:
                pitch = width;
                slice = pitch * (height + ((ulong)(height >> 1) * 4u));
                aSlice = Align((uint)pitch, D3D12_TEXTURE_DATA_PITCH_ALIGNMENT) * (height + ((ulong)(height >> 1) * 4u));
                break;

            default:
                {
                    uint bpp = BitsPerPixel(fmt);

                    if (bpp == 0)
                        return -1;


                    // Default byte alignment
                    pitch = ((ulong)(width) * bpp + 7u) / 8u;
                    slice = pitch * height;
                    aSlice = Align((uint)pitch, D3D12_TEXTURE_DATA_PITCH_ALIGNMENT) * (ulong)(height);
                }
                break;
    }
    
        rowPitch = pitch;
        slicePitch = slice;
        alignedSlicePitch = aSlice;
        return 0;
    }

}
