using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// 中止理由コントローラー
/// </summary>
[ApiController]
[ApiVersion("1")]
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
    /// 中止理由を取得する
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("api/v{version:apiVersion}/cancelReasons")]
    public IActionResult GetCancelReasons()
    {
        _cancelReasonUsecase.GetCancelReasons();
        return Ok();
    }
}
