using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Asp.Versioning;

using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// 受診者コントローラー
/// </summary>
[ApiController]
[ApiVersion("1")]
[Authorize]
public class ExamineeController: ControllerBase
{
    private readonly IExamineeUsecase _examineeUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="examineeUsecase">受診者ユースケース</param>
    public ExamineeController(IExamineeUsecase examineeUsecase)
    {
        _examineeUsecase = examineeUsecase;
    }

    /// <summary>
    /// AP1024_受診者一覧を取得する
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConsultExaminee))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/examinees/placeSchedules/{placeScheduleId}/examMenus/{examMenuId}")]
    public async Task<IActionResult> GetConsultExamineesAsync([FromRoute][Required] Guid placeScheduleId, [FromRoute][Required] int examMenuId,
                                                              [FromQuery][Required] int status)
    {
        var results = await _examineeUsecase.GetConsultExamineesAsync(placeScheduleId, examMenuId, status);
        return Ok(results);
    }
}