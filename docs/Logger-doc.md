# NARLibs.Logging
N4RNAC个人项目用通用库日志系统

## How to use
`NARLibs.Logging.Logger` 提供静态初始化入口和 `Logger.level.Log("message")` 调用方式：

```csharp
using NARLibs.Logging;

Logger.Initialize(new LoggerOptions
{
    WriteToConsole = true,              // 默认 true
    WriteToFile = true,                 // 是否写入文件，默认 false
    LogDirectory = Path.Combine(AppContext.BaseDirectory, "logs"),
    LogDebug = true                     // 是否记录 Trace/Debug，默认 false
});

Logger.Info.Log("程序启动");
Logger.Debug.Log("调试信息");
Logger.Warn.Log("警告");
Logger.Error.Log("错误");
Logger.Fatal.Log("致命错误");

Logger.Shutdown(); // 可选：释放日志文件句柄
```

输出格式固定为：

```text
[2026-11-18 21:22:39][INFO] (程序启动)
```

启用文件输出后，会在 `LogDirectory` 中创建
`log-yyyyMMdd-number.log`（例如 `log-20261118-1.log`）。同一天再次初始化时会使用下一个序号，
例如 `log-20261118-2.log`；跨天后自动切换到新日期的文件。`LogDebug = false` 时，
`Logger.Trace` 和 `Logger.Debug` 不会输出。

> 日志目录相对于初始化时的当前工作目录解析；建议传入绝对路径。初始化可以重复调用，
> 新配置会替换旧配置并关闭旧的日志文件。
