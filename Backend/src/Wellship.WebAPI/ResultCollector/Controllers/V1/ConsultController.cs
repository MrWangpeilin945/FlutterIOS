using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// 受診コントローラー
/// </summary>
[ApiController]
[ApiVersion("1")]
public class ConsultController : ControllerBase
{
    private readonly IConsultUsecase _consultUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="consultUsecase">受診ユースケース</param>
    public ConsultController(IConsultUsecase consultUsecase)
    {
        _consultUsecase = consultUsecase;
    }

    /// <summary>
    /// 受診番号の存在を確認する
    /// </summary>
    /// <param name="consultNumberRequest">受診番号リクエスト</param>
    /// <returns>存在するか</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost]
    [Route("api/v{version:apiVersion}/consult/consultNumber/verify")]
    public async Task<IActionResult> VerifyConsultNumberAsync([FromBody] ConsultNumberRequest consultNumberRequest)
    {
        // NOTE: 受診番号が存在しない場合はUsecaseで例外発生
        await _consultUsecase.VerifyConsultNumberAsync(consultNumberRequest);
        return Ok();
    }

    /// <summary>
    /// 未受診の検査メニューを取得する
    /// </summary>
    /// <param name="consultNumber">受診番号</param>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UnexaminedMenuList))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/consult/{consultNumber}/unexaminedMenus")]
    public async Task<IActionResult> GetUnexaminedMenus([FromRoute][Required] string consultNumber)
    {
        var results = await _consultUsecase.GetUnexaminedMenusAsync(consultNumber);
        return Ok(results);
    }

    /// <summary>
    /// 簡易な受診者情報を取得する
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("api/v{version:apiVersion}/consult/{consultNumber}/simple")]
    public IActionResult GetSimpleExaminee()
    {
        _consultUsecase.GetSimpleExaminee();
        return Ok();
    }

    /// <summary>
    /// 検査内容を取得する
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamContent))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/consult/{consultNumber}/examItems")]
    public IActionResult GetExamItemsExaminee([FromQuery][Required] int examMenuId)
    {
        _consultUsecase.GetExamItemsExaminee();
        return Ok();
    }

    /// <summary>
    /// 検査の実施有無と中止理由を登録する
    /// </summary>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost]
    [Route("api/v{version:apiVersion}/consult/{consultNumber}/executions")]
    public async Task<IActionResult> RegisterExecutions([FromRoute] string consultNumber, [FromBody] ExecutionsRequest executions)
    {
        await _consultUsecase.RegisterExecutionsAsync(consultNumber, executions);
        return Ok();
    }

    /// <summary>
    /// 検査結果入力情報を取得する
    /// </summary>
    /// <param name="consultNumber">受診番号</param>
    /// <param name="examMenuId">検査メニューID</param>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InputExamItems))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/consult/{consultNumber}/inputExamItems")]
    public async Task<IActionResult> GetInputExamItemsExamineeAsync([FromRoute][Required] string consultNumber, [FromQuery][Required] int examMenuId)
    {
        var results = await _consultUsecase.GetInputExamItemsExamineeAsync(consultNumber, examMenuId);
        return Ok(results);
    }

    /// <summary>
    /// 前提検査メニューを検証する（仮：動作確認用）
    /// </summary>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InputExamItems))]
    [HttpGet]
    [Route("api/v{version:apiVersion}/consult/{consultNumber}/prior/{currentExamId}")]
    public async Task<IActionResult> ValidatePriorExamMenus([FromRoute][Required] string consultNumber, [FromRoute][Required] int currentExamId)
    {
        // TODO: 検証ロジックを実装後に削除すること
        var results = await _consultUsecase.ValidatePriorExamMenus(consultNumber, currentExamId);
        return Ok(results);
    }

    /// <summary>
    /// 検査結果相関ルールを検証する（仮：動作確認用）
    /// </summary>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost]
    [Route("api/v{version:apiVersion}/consult/{consultNumber}/correlation")]
    public async Task<IActionResult> ValidateCorrelationRuleAsync([FromRoute][Required] string consultNumber, [FromBody][Required] ResultsRequest request)
    {
        // TODO: 検証ロジックを実装後に削除すること
        var results = await _consultUsecase.ValidateCorrelationRuleAsync(consultNumber, request);
        return Ok(results);
    }
}
