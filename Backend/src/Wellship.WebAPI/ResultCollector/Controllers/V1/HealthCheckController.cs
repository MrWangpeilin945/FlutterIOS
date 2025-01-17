using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

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

    private const string bucketName = "wellship-stg-s3-fileshare-ryobi"; // TODO: 仮テスト用
    private const string keyName = "Test/dummy.txt"; // TODO: 仮テスト用
    private static readonly RegionEndpoint BucketRegion = RegionEndpoint.APNortheast1; // TODO: 仮テスト要

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

    /// <summary>
    /// AmazonS3に置かれた特定のファイルの内容を取得します。
    /// 権限周りのテスト用であり、本実装のときに消します。 
    /// </summary>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/health/s3-dummy-text")]
    public async Task<IActionResult> GetS3TextAsync()
    {
        // TODO: 動作検証のための仮メソッドです。あとから消すこと！

        // TODO: 本当はIAmazonS3をDIしないといけないはず。
        var s3Client = new AmazonS3Client(BucketRegion);

        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = keyName
        };

        using (var response = await s3Client.GetObjectAsync(request))
        using (var responseStream = response.ResponseStream)
        using (var reader = new StreamReader(responseStream))
        {
            var content = await reader.ReadToEndAsync();
            return Ok(content);
        }
    }
}
