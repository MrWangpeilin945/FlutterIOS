using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// 動作確認用コントローラ
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
    /// <param name="thresholdUsecaseUsecase">基準パターンユースケース</param>
    public ThresholdController(IThresholdUsecase thresholdUsecaseUsecase)
    {
        _thresholdUsecaseUsecase = thresholdUsecaseUsecase;
    }

    /// <summary>
    /// EC2014_基準パターンを登録する
    /// </summary>
    /// <param name="request">連携データ</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ErrorObject))] 
    [HttpPost]
    [Route("api/v{version:apiVersion}/external/thresholds")]
    public async Task<IActionResult> StorePlaceSchedulesAsync([FromBody] Threshold[] request)
    {
        var result = await _thresholdUsecaseUsecase.StoreThresholdsAsync(request.ToList());

        return Ok(result);
    }
}
