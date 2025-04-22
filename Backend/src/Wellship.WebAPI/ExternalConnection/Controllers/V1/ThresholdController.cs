using System.Text.Json;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Controllers.V1;
/// <summary>
/// 基準パターン コントローラ
/// </summary>
[ApiController]
[ApiVersion("1")]
[OpenApiIgnore]
public class ThresholdController : ControllerBase
{
    private readonly IThresholdUsecase _thresholdUsecaseUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="thresholdUsecaseUsecase">基準パターン ユースケース</param>
    public ThresholdController(IThresholdUsecase thresholdUsecaseUsecase)
    {
        _thresholdUsecaseUsecase = thresholdUsecaseUsecase;
    }

    /// <summary>
    /// EC2014_基準パターンを登録する
    /// </summary>
    /// <param name="request">連携データ</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status207MultiStatus, Type = typeof(ErrorObject))]
    [HttpPost]
    [Route("api/v{apiVersion}/external/thresholds")]
    public async Task<IActionResult> StoreThresholdsAsync([FromBody] Threshold[] request)
    {
        var result = await _thresholdUsecaseUsecase.StoreThresholdsAsync(request.ToList());
        if (result.Count > 0)
        {
            return new ContentResult
            {
                Content = JsonSerializer.Serialize(result),
                ContentType = "application/json",
                StatusCode = StatusCodes.Status207MultiStatus,
            };
        }
        else
        {
            return Ok();
        }
    }
}
