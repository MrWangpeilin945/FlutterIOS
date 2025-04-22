using System.Text.Json;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Controllers.V1;
/// <summary>
/// 会場日程 コントローラ
/// </summary>
[ApiController]
[ApiVersion("1")]
[OpenApiIgnore]
public class PlaceScheduleController : ControllerBase
{
    private readonly IPlaceScheduleUsecase _placeScheduleUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="placeScheduleUsecases">会場日程 ユースケース</param>
    public PlaceScheduleController(IPlaceScheduleUsecase placeScheduleUsecases)
    {
        _placeScheduleUsecase = placeScheduleUsecases;
    }

    /// <summary>
    /// EC2012_会場日程を登録する
    /// </summary>
    /// <param name="request">連携データ</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status207MultiStatus, Type = typeof(ErrorObject))]
    [HttpPost]
    [Route("api/v{version:apiVersion}/external/placeSchedules")]
    public async Task<IActionResult> StorePlaceSchedulesAsync([FromBody] PlaceSchedule[] request)
    {
        var result = await _placeScheduleUsecase.StorePlaceSchedulesAsync(request.ToList());

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
