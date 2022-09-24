using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace BlockyWeb.Tests;

public class SimpleLogger<T> : ILogger<T>
{
    readonly ITestOutputHelper _outputHelper;

    public SimpleLogger(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }
    
    public IDisposable BeginScope<TState>(TState state)
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _outputHelper.WriteLine($"{logLevel} {eventId} {state}");
    }
}