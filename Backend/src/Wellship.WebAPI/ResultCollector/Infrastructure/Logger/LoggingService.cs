namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Logger;

/// <summary>
/// ロギングサービス
/// </summary>
public sealed class LoggingService : ILoggingService
{
    private readonly ILogger<LoggingService> _logger;
    private readonly ITenantProvider _tenantProvider;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public LoggingService(ILogger<LoggingService> logger, ITenantProvider tenantProvider)
    {
        _logger = logger;
        _tenantProvider = tenantProvider;
    }

    // TODO: ここのログ内容とインターフェースは再考したい。
    // メッセージだけじゃなくて拡張性を持たせたいわね
    /// <inheritdoc/>
    public void Log(string message)
    {
        // 試しにテナントキーを出力
        _logger.LogError($"テナントキー: {_tenantProvider.TenantKey}, メッセージ: {message}");
    }
}
