using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace FF16PackLib.GUI;

public class TextRedirectLoggerProvider : ILoggerProvider
{
    private TextRedirector _redirector;
    
    public TextRedirectLoggerProvider(TextRedirector redirector)
    {
        _redirector = redirector;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new TextRedirectLogger(_redirector);
    }

    public void Dispose()
    {
        
    }
}
