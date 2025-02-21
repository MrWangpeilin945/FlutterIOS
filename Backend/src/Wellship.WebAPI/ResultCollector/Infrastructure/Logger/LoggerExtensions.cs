using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Utilities;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Logger;

/// <summary>
/// カスタムの詳細情報を含むログを記録するための拡張メソッドを提供します。
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// カスタムの詳細情報を含むをログに記録します。
    /// ILoggerを拡張してlogEventInfoを扱えるようにします。
    /// </summary>
    /// <param name="logger">ロガーインスタンス</param>
    /// <param name="logLevel">ログレベル</param>
    /// <param name="errorCode">エラーコード</param>
    /// <param name="message">メッセージ</param>
    /// <param name="tenantKey">テナントキー</param>
    public static void LogWithDetails(this ILogger logger, LogLevel logLevel, ErrorCode errorCode, string message, string tenantKey)
    {
        var logEventInfo = new NLog.LogEventInfo(logLevel.ToNLogLevel(), logger.GetType().FullName, message);
        logEventInfo.Properties["TenantKey"] = tenantKey;

        if (errorCode != ErrorCode.None)
        {
            logEventInfo.Properties["ErrorCode"] = errorCode.ToString();
        }

        var nlogger = NLog.LogManager.GetLogger(logger.GetType().FullName);
        nlogger.Log(logEventInfo);
    }
}
