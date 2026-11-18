namespace NARLibs.Logging;

/// <summary>
/// Configuration used when initializing <see cref="Logger"/>.
/// </summary>
public sealed class LoggerOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether log entries are written to the console.
    /// The default is <see langword="true"/>.
    /// </summary>
    public bool WriteToConsole { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether log entries are written to a file.
    /// The default is <see langword="false"/>.
    /// </summary>
    public bool WriteToFile { get; set; }

    /// <summary>
    /// Gets or sets the directory in which log files are created.
    /// The default is a <c>logs</c> directory under the current working directory.
    /// </summary>
    /// <remarks>
    /// A file named <c>log-yyyyMMdd-number.log</c> is created in this directory when file logging is enabled.
    /// Relative paths are resolved against the current working directory during initialization.
    /// </remarks>
    public string LogDirectory { get; set; } = "logs";

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="Microsoft.Extensions.Logging.LogLevel.Debug"/>
    /// and <see cref="Microsoft.Extensions.Logging.LogLevel.Trace"/> entries are recorded.
    /// The default is <see langword="false"/>, so only information and higher levels are recorded.
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
