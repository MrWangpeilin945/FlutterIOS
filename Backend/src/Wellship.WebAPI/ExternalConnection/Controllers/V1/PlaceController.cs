using System.Text.Json;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Controllers.V1;
/// <summary>
/// 会場 コントローラ
/// </summary>
[ApiController]
[ApiVersion("1")]
[OpenApiIgnore]
public class PlaceController : ControllerBase
{
    private readonly IPlaceUsecase _placeUsecaseUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="placeUsecaseUsecase">会場 ユースケース</param>
    public PlaceController(IPlaceUsecase placeUsecaseUsecase)
    {
        _placeUsecaseUsecase = placeUsecaseUsecase;
    }

    /// <summary>
    /// EC2007_会場を登録する
    /// </summary>
    /// <param name="request">連携データ</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status207MultiStatus, Type = typeof(ErrorObject))]
    [HttpPost]
    [Route("api/v{version:apiVersion}/external/places")]
    public async Task<IActionResult> StorePlacesAsync([FromBody] Place[] request)
    {
        var result = await _placeUsecaseUsecase.StorePlacesAsync(request.ToList());

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
