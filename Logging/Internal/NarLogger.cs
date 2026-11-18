using Microsoft.Extensions.Logging;

namespace NARLibs.Logging.Internal;

internal sealed class NarLogger : ILogger
{
    private readonly LoggerOptions _options;
    private readonly LogSink _sink;

    public NarLogger(LoggerOptions options, LogSink sink)
    {
        _options = options;
        _sink = sink;
    }

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
        => null;

    public bool IsEnabled(LogLevel logLevel)
    {
        if (logLevel == LogLevel.None)
        {
            return false;
        }

        if (!_options.LogDebug && logLevel < LogLevel.Information)
        {
            return false;
        }

        return _options.WriteToConsole || _options.WriteToFile;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(formatter);
        _sink.Write(logLevel, formatter(state, exception));
    }
}
