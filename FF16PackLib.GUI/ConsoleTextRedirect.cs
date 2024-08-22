using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FF16PackLib.GUI
{

    public class ConsoleTextRedirect : TextWriter
    {
        public string text = "";

        public ConsoleTextRedirect() { }

        public void Clear() => text = "";

        public override void Write(char value) => text += value;

        public override void Write(string value) => text += value;

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
