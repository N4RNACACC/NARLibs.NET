using Microsoft.Extensions.Logging;

namespace NARLibs.Logging.Internal;

internal sealed class NarLoggerProvider : ILoggerProvider
{
    private readonly LoggerOptions _options;
    private readonly LogSink _sink;
    private readonly object _sync = new();
    private bool _disposed;

    public NarLoggerProvider(LoggerOptions options)
    {
        _options = options;
        _sink = new LogSink(options);
    }

    public ILogger CreateLogger(string categoryName)
    {
        ArgumentNullException.ThrowIfNull(categoryName);

        lock (_sync)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return new NarLogger(_options, _sink);
        }
    }

    public void Dispose()
    {
        lock (_sync)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _sink.Dispose();
        }
    }
}
