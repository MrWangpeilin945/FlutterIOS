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
    /// <param name="consultNumber">受診番号</param>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost]
    [Route("api/v{version:apiVersion}/consult/{consultNumber}/results")]
    public IActionResult RegisterResults([FromRoute] string consultNumber, [FromBody] ResultsRequest results)
    {
        _resultUsecase.RegisterResults();
        return Ok();
    }
}
