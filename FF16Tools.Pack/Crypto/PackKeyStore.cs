using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Pack.Crypto;

/// <summary>
/// Pack key registry.
/// </summary>
public class PackKeyStore
{
    /// <summary>
    /// FINAL FANTASY XVI / 16
    /// </summary>
    public const string FFXVI_CODENAME = "faith";

    /// <summary>
    /// FINAL FANTASY TACTICS - The Ivalice Chronicles
    /// </summary>
    public const string FFT_IVALICE_CODENAME = "ffto";

    public static readonly FrozenDictionary<string, ulong> Keys = new Dictionary<string, ulong>()
    {
        [FFXVI_CODENAME] = 0x49D18FC870F3824E,
        [FFT_IVALICE_CODENAME] = 0x534C8F9A54D612E6,
    }.ToFrozenDictionary();
}
