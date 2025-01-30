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
        await LogAsync(LogLevel.Information, message, details);
    }

    /// <inheritdoc/>
    public async Task LogWarnAsync(string message, Dictionary<string, object>? details = null)
    {
        await LogAsync(LogLevel.Warning, message, details);
    }

    /// <inheritdoc/>
    public async Task LogErrorAsync(string message, Dictionary<string, object>? details = null)
    {
        await LogAsync(LogLevel.Error, message, details);

        // アラート通知用のためNLog経由で標準出力
        // TODO: テナントキーを構造化ログに出す（CloudWatchのアラート通知フィルタ用）
        var tenantKey = _tenantProvider.TenantKey;
        _logger.LogError(message);
    }

    private async Task LogAsync(LogLevel level, string message, Dictionary<string, object>? details)
    {
        var log = new AppLog()
        {
            LogLevel = level,
            Message = message,
            Details = details
        };

        // データベースに書き込み
        await _logRepository.WriteLogAsync(log);
    }
}
