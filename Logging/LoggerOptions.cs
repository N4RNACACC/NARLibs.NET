namespace NARLibs.Logging;

/// <summary>
/// 初始化 <see cref="Logger"/> 时使用的配置。
/// </summary>
public sealed class LoggerOptions
{
    /// <summary>
    /// 获取或设置一个值，指示是否将日志写入控制台。
    /// 默认值为 <see langword="true"/>。
    /// </summary>
    public bool WriteToConsole { get; set; } = true;

    /// <summary>
    /// 获取或设置一个值，指示是否将日志写入文件。
    /// 默认值为 <see langword="false"/>。
    /// </summary>
    public bool WriteToFile { get; set; }

    /// <summary>
    /// 获取或设置创建日志文件所在的目录。
    /// 默认值为当前工作目录下的 <c>logs</c> 目录。
    /// </summary>
    /// <remarks>
    /// 启用文件日志记录后，会在此目录中创建名为 <c>log-yyyyMMdd-number.log</c> 的日志文件。
    /// 相对路径会在初始化时基于当前工作目录解析。
    /// </remarks>
    public string LogDirectory { get; set; } = "logs";

    /// <summary>
    /// 获取或设置一个值，指示是否记录
    /// <see cref="Microsoft.Extensions.Logging.LogLevel.Debug"/> 和
    /// <see cref="Microsoft.Extensions.Logging.LogLevel.Trace"/> 级别的日志。
    /// 默认值为 <see langword="false"/>，因此只记录信息级别及更高级别的日志。
    /// </summary>
    public bool LogDebug { get; set; }

    internal LoggerOptions Clone() => (LoggerOptions)MemberwiseClone();

    internal string ResolveLogDirectory()
    {
        if (string.IsNullOrWhiteSpace(LogDirectory))
        {
            throw new InvalidOperationException(
                $"{nameof(LogDirectory)} must be set when {nameof(WriteToFile)} is enabled.");
        }

        return Path.GetFullPath(LogDirectory);
    }
}
