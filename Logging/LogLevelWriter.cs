using Microsoft.Extensions.Logging;

namespace NARLibs.Logging;

/// <summary>
/// 通过 <see cref="Logger"/> 按固定 <see cref="LogLevel"/> 写入消息。
/// </summary>
public sealed class LogLevelWriter
{
    private readonly LogLevel _level;

    internal LogLevelWriter(LogLevel level)
    {
        _level = level;
    }

    /// <summary>
    /// 按此写入器所代表的级别写入 <paramref name="message"/>。
    /// </summary>
    /// <param name="message">消息文本。该文本不会被当作格式字符串处理。</param>
    public void Log(string message) => Logger.Write(_level, message);
}
