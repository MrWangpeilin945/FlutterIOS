using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// 会場日程コントローラー
/// </summary>
[ApiController]
[ApiVersion("1")]
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlaceScheduleTeams))]
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

    /// <summary>
    /// 班を指定して会場日程を取得する
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("api/v{version:apiVersion}/placeSchedules/places")]
    public IActionResult GetTeamPlaceSchedules()
    {
        _placeScheduleUsecase.GetTeamPlaceSchedules();
        return Ok();
    }

    /// <summary>
    /// 会場状況を取得する
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("api/v{version:apiVersion}/placeSchedules/placeScheduleClosingStatus")]
    public IActionResult GetPlaceScheduleLockingStatus()
    {
        _placeScheduleUsecase.GetPlaceScheduleLockingStatus();
        return Ok();
    }

    /// <summary>
    /// 会場状況を更新する
    /// </summary>
    /// <returns></returns>
    [HttpPut]
    [Route("api/v{version:apiVersion}/placeSchedules/placeScheduleClosingStatus")]
    public IActionResult UpdatePlaceScheduleLockingStatus()
    {
        _placeScheduleUsecase.UpdatePlaceScheduleLockingStatus();
        return Ok();
    }
}
