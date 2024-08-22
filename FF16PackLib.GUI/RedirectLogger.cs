using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16PackLib.GUI;

public class TextRedirectLogger : ILogger
{
    private TextRedirector _redirector;

    public TextRedirectLogger(TextRedirector textRedirector)
    {
        _redirector = textRedirector;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _redirector.WriteLine(state);
    }
}
