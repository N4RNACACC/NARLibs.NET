using Microsoft.Extensions.Logging;

namespace NARLibs.Logging;

/// <summary>
/// Writes messages at a fixed <see cref="LogLevel"/> through <see cref="Logger"/>.
/// </summary>
public sealed class LogLevelWriter
{
    private readonly LogLevel _level;

    internal LogLevelWriter(LogLevel level)
    {
        _level = level;
    }

    /// <summary>
    /// Writes <paramref name="message"/> at the level represented by this writer.
    /// </summary>
    /// <param name="message">The message text. It is not treated as a format string.</param>
    public void Log(string message) => Logger.Write(_level, message);
}
