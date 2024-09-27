using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// 会場日程コントローラー
/// </summary>
[ApiController]
[ApiVersion("1.0")]
public class PlaceScheduleController : ControllerBase
{
    private readonly IPlaceScheduleUsecase _placeScheduleUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="placeScheduleUsecase">会場日程ユースケース</param>
    public PlaceScheduleController(IPlaceScheduleUsecase placeScheduleUsecase)
    {
        _placeScheduleUsecase = placeScheduleUsecase;
    }

    /// <summary>
    /// 日付を指定して班と会場のリストを取得する
    /// </summary>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PlaceScheduleTeams>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/placeSchedules/teams")]
    public ActionResult<IEnumerable<PlaceScheduleTeams>> GetTeams([FromQuery] string date)
    {

        if (!DateOnly.TryParse(date, out var dateOnlyDate))
        {
            return BadRequest("日付の形式が無効です。yyyy-MM-ddの形式で日付を指定してください。");
        }

        var results = _placeScheduleUsecase.GetTeams(dateOnlyDate);
        return Ok(results);
    }
}
