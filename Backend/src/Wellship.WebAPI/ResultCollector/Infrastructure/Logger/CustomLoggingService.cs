using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Logger;

/// <summary>
/// カスタムロギングサービスのインターフェース
/// アプリケーションから明示的に記録したいときに使います。
/// データベースに書き込みます。
/// ログレベルがERRORの場合はNLog経由でも出力します。
/// </summary>
public sealed class CustomLoggingService : ICustomLoggingService
{
    private readonly ILogger<CustomLoggingService> _logger;
    private readonly ITenantProvider _tenantProvider;
    private readonly ILogRepository _logRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public CustomLoggingService(ILogger<CustomLoggingService> logger, ITenantProvider tenantProvider, ILogRepository logRepository)
    {
        _logger = logger;
        _tenantProvider = tenantProvider;
        _logRepository = logRepository;
    }

    /// <inheritdoc/>
    public async Task LogInfoAsync(string message, Dictionary<string, object>? details = null)
    {
        // アラート通知用のためNLog経由で標準出力
        // TODO: テナントキーを構造化ログに出す（CloudWatchのアラート通知フィルタ用）
        var tenantKey = _tenantProvider.TenantKey;

        await LogAsync(LogLevel.Information, ErrorCode.None, message, details);
        _logger.LogWithDetails(LogLevel.Information, ErrorCode.None, message, tenantKey);
    }

    /// <inheritdoc/>
    public async Task LogWarnAsync(string message, ErrorCode errorCode, Dictionary<string, object>? details = null)
    {
        // アラート通知用のためNLog経由で標準出力
        // TODO: テナントキーを構造化ログに出す（CloudWatchのアラート通知フィルタ用）
        var tenantKey = _tenantProvider.TenantKey;

        await LogAsync(LogLevel.Warning, errorCode, message, details);
        _logger.LogWithDetails(LogLevel.Warning, errorCode, message, tenantKey);
    }

    /// <inheritdoc/>
    public async Task LogErrorAsync(string message, ErrorCode errorCode, Dictionary<string, object>? details = null)
    {

        // アラート通知用のためNLog経由で標準出力
        // TODO: テナントキーを構造化ログに出す（CloudWatchのアラート通知フィルタ用）
        var tenantKey = _tenantProvider.TenantKey;

        await LogAsync(LogLevel.Error, errorCode, message, details);
        _logger.LogWithDetails(LogLevel.Error, errorCode, message, tenantKey);
    }

    /// <summary>
    /// データベースにログを書き込みます。
    /// </summary>
    private async Task LogAsync(LogLevel level, ErrorCode errorCode, string message, Dictionary<string, object>? details)
    {
        // detailsにerrorCodeを追加。ただしdetailsにErrorCodeがキーの要素があれば上書きする
        if (errorCode != ErrorCode.None)
        {
            details ??= [];
            details["ErrorCode"] = errorCode.ToString();
        }

        var log = new AppLog()
        {
            LogLevel = level,
            Message = message,
            Details = details
        };

        // データベースに書き込む
        await _logRepository.WriteLogAsync(log);
    }
}
