using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Asp.Versioning;

using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// 進捗コントローラー
/// </summary>
[ApiController]
[ApiVersion("1")]
[Authorize]
public class ProgressController : ControllerBase
{
    private readonly IProgressUsecase _progressUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="progressUsecase">進捗ユースケース</param>
    public ProgressController(IProgressUsecase progressUsecase)
    {
        _progressUsecase = progressUsecase;
    }

    /// <summary>
    /// AP1015_進捗状況を取得する
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlaceScheduleProgress))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/progress/{placeScheduleId}")]
    public async Task<IActionResult> GetProgress([FromRoute][Required] Guid placeScheduleId)
    {
        var results = await _progressUsecase.GetProgressAsync(placeScheduleId);
        return Ok(results);
    }
}
