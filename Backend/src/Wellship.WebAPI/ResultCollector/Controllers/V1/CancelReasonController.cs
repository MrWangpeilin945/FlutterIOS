using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// 中止理由コントローラー
/// </summary>
[ApiController]
[ApiVersion("1")]
[Authorize]
public class CancelReasonController : ControllerBase
{
    private readonly ICancelReasonUsecase _cancelReasonUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="cancelReasonUsecase">中止理由ユースケース</param>
    public CancelReasonController(ICancelReasonUsecase cancelReasonUsecase)
    {
        _cancelReasonUsecase = cancelReasonUsecase;
    }

    /// <summary>
    /// AP1011_中止理由一覧を取得する
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CancelReasonList))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/cancelReasons")]
    public async Task<IActionResult> GetCancelReasonsAsync()
    {
        var results = await _cancelReasonUsecase.GetCancelReasonsAsync();
        return Ok(results);
    }
}
