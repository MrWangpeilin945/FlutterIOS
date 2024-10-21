using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// 検査結果コントローラー
/// </summary>
[ApiController]
[ApiVersion("1")]
public class ResultController : ControllerBase
{
    private readonly IResultUsecase _resultUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="resultUsecase">検査結果ユースケース</param>
    public ResultController(IResultUsecase resultUsecase)
    {
        _resultUsecase = resultUsecase;
    }

    /// <summary>
    /// 検査結果を検証する
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("api/v{version:apiVersion}/consult/{consultId}/results/verify")]
    public IActionResult VerifyResults()
    {
        _resultUsecase.VerifyResults();
        return Ok();
    }

    /// <summary>
    /// 検査結果を登録する
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("api/v{version:apiVersion}/consult/{consultId}/results")]
    public IActionResult RegisterResults()
    {
        _resultUsecase.RegisterResults();
        return Ok();
    }
}
