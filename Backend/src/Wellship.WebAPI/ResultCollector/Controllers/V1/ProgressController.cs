using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// 進捗コントローラー
/// </summary>
[ApiController]
[ApiVersion("1.0")]
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
    /// 進捗状況を取得する
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("api/v{version:apiVersion}/progress/{placeScheduleId}")]
    public IActionResult GetProgress()
    {
        _progressUsecase.GetProgress();
        return Ok();
    }
}
