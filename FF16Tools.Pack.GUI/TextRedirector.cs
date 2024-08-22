using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FF16Tools.Pack.GUI;

public class TextRedirector : TextWriter
{
    public string text = "";

    public TextRedirector() { }

    public void Clear() => text = "";

    public override void Write(char value) => text += value;

    public override void Write(string value) => text += value;

    public override Encoding Encoding
    {
        get { return Encoding.ASCII; }
    }
}
