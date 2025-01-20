using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// ヘルスチェックコントローラー
/// </summary>
[ApiController]
[ApiVersion("1")]
public class HealthCheckController : ControllerBase
{
    private readonly IHealthCheckRepository _healthCheckRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="healthCheckRepository">ヘルスチェックリポジトリ</param>
    public HealthCheckController(IHealthCheckRepository healthCheckRepository)
    {
        _healthCheckRepository = healthCheckRepository;
    }

    /// <summary>
    /// アプリケーション起動の正常性を確認する。
    /// </summary>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/health/app")]
    public IActionResult GetHealth()
    {
        return Ok();
    }

    /// <summary>
    /// データベース接続の正常性を確認する。
    /// </summary>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/health/database")]
    public async Task<IActionResult> GetDeepHealthAsync()
    {
        var healthy = await _healthCheckRepository.CheckDatabaseConnectionAsync();

        if (healthy)
        {
            return Ok();
        }

        return StatusCode(503);
    }
}
