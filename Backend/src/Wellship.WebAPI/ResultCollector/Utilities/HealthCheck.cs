using Microsoft.Extensions.Diagnostics.HealthChecks;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Utilities;

/// <summary>
/// ヘルスチェック
/// </summary>
public class HealthCheck : IHealthCheck
{
    private readonly IHealthCheckRepository _healthCheckRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="healthCheckRepository">ヘルスチェックリポジトリ</param>
    public HealthCheck(IHealthCheckRepository healthCheckRepository)
    {
        _healthCheckRepository = healthCheckRepository;
    }

    /// <summary>
    /// ヘルスチェックします。
    /// </summary>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var isHealthy = await _healthCheckRepository.CheckDatabaseConnectionAsync();
        if (isHealthy)
        {
            return HealthCheckResult.Healthy("A healthy result.");
        }

        return new HealthCheckResult(context.Registration.FailureStatus, "An unhealthy result.");
    }
}
