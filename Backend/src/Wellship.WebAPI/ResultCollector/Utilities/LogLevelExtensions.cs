namespace Ryobi.Wellship.WebAPI.ResultCollector.Utilities;

/// <summary>
/// Microsoft.Extensions.Logging.LogLevelの拡張メソッドです。
/// </summary>
public static class LogLevelExtensions
{
    /// <summary>
    /// NLogのログレベルに変換します。
    /// </summary>
    /// <param name="logLevel">変換するMicrosoft.Extensions.Logging.LogLevel</param>
    public static NLog.LogLevel ToNLogLevel(this LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => NLog.LogLevel.Trace,
            LogLevel.Debug => NLog.LogLevel.Debug,
            LogLevel.Information => NLog.LogLevel.Info,
            LogLevel.Warning => NLog.LogLevel.Warn,
            LogLevel.Error => NLog.LogLevel.Error,
            LogLevel.Critical => NLog.LogLevel.Fatal,
            LogLevel.None => NLog.LogLevel.Off,
            _ => NLog.LogLevel.Debug,
        };
    }
}
