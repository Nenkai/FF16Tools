using FF16Tools.Pack.Crypto;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Pack.Packing;

public class PackBuildOptions
{
    /// <summary>
    /// Codename to pack for. Used to determine the encryption key to use.<br/>
    /// Valid codenames are 'faith' (FFXVI), 'ffto' (FFT).<br/>
    /// <br/>
    /// Defaults to 'faith' (FF16).
    /// </summary>
    public string CodeName { get; set; } = PackKeyStore.FFXVI_CODENAME;
 
    private string? _name;
    public string? Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrEmpty(value))
                return;

            if (Encoding.UTF8.GetByteCount(value) > 255)
                throw new ArgumentException("Name of pack must not be longer than 255 bytes.");

            _name = value.Replace('\\', '/');
        }
    }

    public bool Encrypt { get; set; }
    public bool Compress { get; set; }
}
