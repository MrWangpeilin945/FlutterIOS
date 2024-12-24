using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Utilities;

/// <summary>
/// ヘルスチェック
/// </summary>
public class HealthCheck : IHealthCheck
{
    /// <summary>
    /// ヘルスチェックします。
    /// アプリケーションが起動しているかどうかを確認します。
    /// </summary>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(HealthCheckResult.Healthy("A healthy result."));
    }
}
