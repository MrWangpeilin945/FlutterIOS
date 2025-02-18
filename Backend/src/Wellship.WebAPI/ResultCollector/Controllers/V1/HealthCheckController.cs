using Microsoft.AspNetCore.Mvc;

using Asp.Versioning;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.APIModels.Requests;

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
    /// S3イベントの中身を確認する
    /// </summary>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost]
    [Route("/api/v{version:apiVersion}/s3Event")]
    public IActionResult VerifyS3Event([FromBody] S3EventRequest request)
    {
        // TODO: 検証用のため後ほど削除
        Console.WriteLine($"BucketName: {request.BucketName}");
        Console.WriteLine($"ObjectKey: {request.ObjectKey}");
        return Ok();
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
