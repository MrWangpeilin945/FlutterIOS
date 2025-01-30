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
public class PlaceController : ControllerBase
{
    private readonly IPlaceUsecase _administratorUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="placeUsecaseUsecase">会場ユースケース</param>
    public PlaceController(IPlaceUsecase placeUsecaseUsecase)
    {
        _administratorUsecase = placeUsecaseUsecase;
    }

    /// <summary>
    /// EC2007_会場を登録する
    /// </summary>
    /// <param name="request">連携データ</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ErrorObject))]
    [HttpPost]
    [Route("api/v{version:apiVersion}/external/place")]
    public async Task<IActionResult> StorePlacesAsync([FromBody] Place[] request)
    {
        var result = await _administratorUsecase.StorePlacesAsync(request.ToList());

        return Ok(result);
    }
}
