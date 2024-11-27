using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.APIModels.Requests;
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/placeSchedules/teams")]
    public async Task<IActionResult> GetTeamsAsync([FromQuery] string date)
    {

        if (!DateOnly.TryParse(date, out var dateOnlyDate))
        {
            return BadRequest("日付の形式が無効です。yyyy-MM-ddの形式で日付を指定してください。");
        }

        var results = await _placeScheduleUsecase.GetTeamsAsync(dateOnlyDate);
        return Ok(results);
    }

    /// <summary>
    /// 班を指定して会場日程を取得する
    /// </summary>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlaceSchedulePlaces))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/placeSchedules/places")]
    public async Task<IActionResult> GetTeamPlaceSchedulesAsync([FromQuery] string date, [FromQuery] int teamId)
    {
        if (!DateOnly.TryParse(date, out var dateOnlyDate))
        {
            return BadRequest("日付の形式が無効です。yyyy-MM-ddの形式で日付を指定してください。");
        }

        var results = await _placeScheduleUsecase.GetTeamPlaceSchedulesAsync(dateOnlyDate, teamId);
        return Ok(results);
    }

    /// <summary>
    /// 会場ロック状態を取得する
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlaceScheduleLocking))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/placeSchedules/{placeScheduleId}/placeScheduleLockingStatus")]
    public async Task<IActionResult> GetPlaceScheduleLockingStatusAsync([FromRoute][Required] int placeScheduleId)
    {
        var results = await _placeScheduleUsecase.GetPlaceScheduleLockingStatusAsync(placeScheduleId);
        return Ok(results);
    }

    /// <summary>
    /// 会場ロック状態を更新する
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut]
    [Route("api/v{version:apiVersion}/placeSchedules/{placeScheduleId}/placeScheduleLockingStatus")]
    public async Task<IActionResult> UpdatePlaceScheduleLockingStatusAsync([FromRoute][Required] int placeScheduleId,
                                                          [FromBody] PlaceScheduleLockingRequest placeScheduleLockingRequest)
    {
        if(placeScheduleId != placeScheduleLockingRequest.PlaceScheduleId)
        {
            return BadRequest("パスパラメータとリクエストパラメータが一致しません。");
        }
        await _placeScheduleUsecase.UpdatePlaceScheduleLockingStatusAsync(placeScheduleId, placeScheduleLockingRequest.PlaceScheduleLockingStatus);
        return Ok();
    }
}
