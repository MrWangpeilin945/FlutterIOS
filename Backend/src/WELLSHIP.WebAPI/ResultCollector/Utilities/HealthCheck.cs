using Microsoft.Extensions.Diagnostics.HealthChecks;

using Ryobi.WELLSHIP.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.WELLSHIP.WebAPI.ResultCollector.Utilities;

public class HealthCheck : IHealthCheck
{
    private readonly IHealthCheckRepository _healthCheckRepository;
    public HealthCheck(IHealthCheckRepository healthCheckRepository)
    {
        _healthCheckRepository = healthCheckRepository;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var isHealthy = _healthCheckRepository.CheckDatabaseConnection();

        if (isHealthy)
        {
            return Task.FromResult(
                HealthCheckResult.Healthy("A healthy result."));
        }

        return Task.FromResult(
            new HealthCheckResult(
                context.Registration.FailureStatus, "An unhealthy result."));
    }
}
