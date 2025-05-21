using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Pack.Packing;

public class PackBuildOptions
{
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
