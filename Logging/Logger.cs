using Microsoft.Extensions.Logging;
using NARLibs.Logging.Internal;

namespace NARLibs.Logging;

/// <summary>
/// 提供进程范围内简单易用的日志门面。
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
/// Logger.Info.Log("程序启动。");
/// Logger.Debug.Log("调试信息。");
/// </code>
/// </example>
public static class Logger
{
    private const string CategoryName = "NARLibs";
    private static readonly object SyncRoot = new();

    private static ILoggerFactory? _factory;
    private static NarLoggerProvider? _provider;
    private static ILogger? _logger;

    /// <summary>获取用于写入跟踪级别日志的写入器。</summary>
    public static LogLevelWriter Trace { get; } = new(LogLevel.Trace);

    /// <summary>获取用于写入调试级别日志的写入器。</summary>
    public static LogLevelWriter Debug { get; } = new(LogLevel.Debug);

    /// <summary>获取用于写入信息级别日志的写入器。</summary>
    public static LogLevelWriter Info { get; } = new(LogLevel.Information);

    /// <summary>获取用于写入警告级别日志的写入器。</summary>
    public static LogLevelWriter Warn { get; } = new(LogLevel.Warning);

    /// <summary>获取用于写入错误级别日志的写入器。</summary>
    public static LogLevelWriter Error { get; } = new(LogLevel.Error);

    /// <summary>获取用于写入严重级别日志的写入器。</summary>
    public static LogLevelWriter Fatal { get; } = new(LogLevel.Critical);

    /// <summary>
    /// 使用指定配置初始化日志记录器。再次调用此方法会替换原有配置，
    /// 并关闭原有的日志文件句柄。
    /// </summary>
    /// <param name="options">
    /// 要使用的配置。为 <see langword="null"/> 时使用默认配置：
    /// 启用控制台输出、禁用文件输出并禁用调试日志。
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// 已启用文件日志记录，但 <see cref="LoggerOptions.LogDirectory"/> 为空。
    /// </exception>
    /// <exception cref="IOException">无法创建日志目录或日志文件。</exception>
    /// <exception cref="UnauthorizedAccessException">没有访问日志目录的权限。</exception>
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
    /// 停止日志记录器并释放其日志文件句柄。之后的日志调用会使用默认配置重新初始化，
    /// 因此需要自定义配置时请先调用 <see cref="Initialize"/>。
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
