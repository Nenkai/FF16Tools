using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO.Hashing;
using SysDebug = System.Diagnostics.Debug;

using Microsoft.Extensions.Logging;

using CommunityToolkit.HighPerformance.Buffers;
using CommunityToolkit.HighPerformance;

using Syroot.BinaryData;

using FF16Tools.Hashing;
using FF16Tools.Shared;
using FF16Tools.Pack.Crypto;

namespace FF16Tools.Pack;

/// <summary>
/// FF16 Pack file. (Disposable object)
/// </summary>
public class FF16UnpackOptions
{
    /// <summary>
    /// If not null, only files that match this filter will be unpacked.
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// Whether to include a .path file.
    /// </summary>
    public bool IncludePathFile { get; set; } = true;
}
