using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;

namespace NARLibs.Logging.Internal;

internal sealed class LogSink : IDisposable
{
    private static readonly Encoding Utf8WithoutBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    private readonly LoggerOptions _options;
    private readonly object _sync = new();

    private StreamWriter? _writer;
    private DateOnly _fileDate;
    private bool _disposed;

    public LogSink(LoggerOptions options)
    {
        _options = options;

        if (_options.WriteToFile)
        {
            EnsureFile(DateTime.Now);
        }
    }

    public void Write(LogLevel level, string message)
    {
        var timestamp = DateTime.Now;
        var line = FormatLine(timestamp, level, message);

        lock (_sync)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (_options.WriteToConsole)
            {
                Console.WriteLine(line);
            }

            if (_options.WriteToFile)
            {
                EnsureFile(timestamp);
                _writer!.WriteLine(line);
            }
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
            CloseWriter();
        }
    }

    private static string FormatLine(DateTime timestamp, LogLevel level, string message)
    {
        var time = timestamp.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        return $"[{time}][{GetLevelText(level)}] {message}";
    }

    private static string GetLevelText(LogLevel level) => level switch
    {
        LogLevel.Trace => "TRACE",
        LogLevel.Debug => "DEBUG",
        LogLevel.Information => "INFO",
        LogLevel.Warning => "WARN",
        LogLevel.Error => "ERROR",
        LogLevel.Critical => "FATAL",
        _ => level.ToString().ToUpperInvariant()
    };

    private void EnsureFile(DateTime timestamp)
    {
        var date = DateOnly.FromDateTime(timestamp);
        if (_writer is not null && _fileDate == date)
        {
            return;
        }

        CloseWriter();
        Directory.CreateDirectory(_options.LogDirectory);

        var dateText = timestamp.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        for (var number = 1; ; number++)
        {
            var path = Path.Combine(_options.LogDirectory, $"log-{dateText}-{number}.log");
            if (File.Exists(path))
            {
                continue;
            }

            try
            {
                var stream = new FileStream(
                    path,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.Read);

                _writer = new StreamWriter(stream, Utf8WithoutBom)
                {
                    AutoFlush = true
                };
                _fileDate = date;
                return;
            }
            catch (IOException) when (File.Exists(path))
            {
                // 在检查与创建文件之间，有其他进程创建了相同序号的文件。
            }
        }
    }

    private void CloseWriter()
    {
        _writer?.Dispose();
        _writer = null;
    }
}
