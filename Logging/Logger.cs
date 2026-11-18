using Microsoft.Extensions.Logging;
using NARLibs.Logging.Internal;

namespace NARLibs.Logging;

/// <summary>
/// Provides a process-wide, easy-to-use logging facade.
/// </summary>
/// <example>
/// <code>
/// Logger.Initialize(new LoggerOptions
/// {
///     WriteToFile = true,
///     LogDirectory = "logs",
///     LogDebug = true
/// });
///
/// Logger.Info.Log("Application started.");
/// Logger.Debug.Log("Debug details.");
/// </code>
/// </example>
public static class Logger
{
    private const string CategoryName = "NARLibs";
    private static readonly object SyncRoot = new();

    private static ILoggerFactory? _factory;
    private static NarLoggerProvider? _provider;
    private static ILogger? _logger;

    /// <summary>Gets the writer for trace-level entries.</summary>
    public static LogLevelWriter Trace { get; } = new(LogLevel.Trace);

    /// <summary>Gets the writer for debug-level entries.</summary>
    public static LogLevelWriter Debug { get; } = new(LogLevel.Debug);

    /// <summary>Gets the writer for information-level entries.</summary>
    public static LogLevelWriter Info { get; } = new(LogLevel.Information);

    /// <summary>Gets the writer for warning-level entries.</summary>
    public static LogLevelWriter Warn { get; } = new(LogLevel.Warning);

    /// <summary>Gets the writer for error-level entries.</summary>
    public static LogLevelWriter Error { get; } = new(LogLevel.Error);

    /// <summary>Gets the writer for critical-level entries.</summary>
    public static LogLevelWriter Fatal { get; } = new(LogLevel.Critical);

    /// <summary>
    /// Initializes the logger with the supplied configuration. Calling this method again replaces
    /// the previous configuration and closes its file handle.
    /// </summary>
    /// <param name="options">
    /// The configuration to use. When <see langword="null"/>, a default configuration is used:
    /// console output enabled, file output disabled, and debug logging disabled.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// File logging is enabled but <see cref="LoggerOptions.LogDirectory"/> is empty.
    /// </exception>
    /// <exception cref="IOException">The log directory or log file cannot be created.</exception>
    /// <exception cref="UnauthorizedAccessException">Access to the log directory is denied.</exception>
    public static void Initialize(LoggerOptions? options = null)
    {
        lock (SyncRoot)
        {
            var previousFactory = _factory;
            var previousProvider = _provider;
            ReplaceLogger(options ?? new LoggerOptions());
            try
            {
                previousFactory?.Dispose();
            }
            finally
            {
                previousProvider?.Dispose();
            }
        }
    }

    /// <summary>
    /// Stops the logger and releases its file handle. A later log call initializes the logger again
    /// with default options, so call <see cref="Initialize"/> first when custom options are required.
    /// </summary>
    public static void Shutdown()
    {
        lock (SyncRoot)
        {
            _logger = null;
            var factory = _factory;
            var provider = _provider;
            _factory = null;
            _provider = null;
            try
            {
                factory?.Dispose();
            }
            finally
            {
                provider?.Dispose();
            }
        }
    }

    internal static void Write(LogLevel level, string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        lock (SyncRoot)
        {
            if (_logger is null)
            {
                ReplaceLogger(new LoggerOptions());
            }

            _logger!.Log(level, default, message, null, static (state, _) => state);
        }
    }

    private static void ReplaceLogger(LoggerOptions options)
    {
        var settings = options.Clone();
        if (settings.WriteToFile)
        {
            settings.LogDirectory = settings.ResolveLogDirectory();
        }

        var provider = new NarLoggerProvider(settings);
        ILoggerFactory? factory = null;
        try
        {
            factory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(settings.LogDebug ? LogLevel.Trace : LogLevel.Information);
                builder.AddProvider(provider);
            });

            var logger = factory.CreateLogger(CategoryName);
            _factory = factory;
            _provider = provider;
            _logger = logger;
        }
        catch
        {
            try
            {
                factory?.Dispose();
            }
            finally
            {
                provider.Dispose();
            }

            throw;
        }
    }
}
