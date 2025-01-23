using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Asp.Versioning;

using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// 受診コントローラー
/// </summary>
/// [ApiController]
[ApiVersion("1")]
[Authorize]
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
    /// AP1007_受診番号の存在を確認する
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
    /// AP1008_未受診の検査メニューを取得する
    /// </summary>
    /// <param name="consultNumber">受診番号</param>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UnexaminedMenuList))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/consult/{consultNumber}/unexaminedMenus")]
    public async Task<IActionResult> GetUnexaminedMenus([FromRoute][Required] string consultNumber)
    {
        var results = await _consultUsecase.GetUnexaminedMenusAsync(consultNumber);
        return Ok(results);
    }

    /// <summary>
    /// AP1010_検査内容を取得する
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamContent))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/consult/{consultNumber}/examItems")]
    public async Task<IActionResult> GetExamItemsExaminee([FromRoute][Required] string consultNumber, [FromQuery][Required] int examMenuId)
    {
        var results = await _consultUsecase.GetExamItemsExamineeAsync(consultNumber, examMenuId);
        return Ok(results);
    }

    /// <summary>
    /// AP1022_検査の実施有無と中止理由を登録する
    /// </summary>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost]
    [Route("api/v{version:apiVersion}/consult/{consultNumber}/executions")]
    public async Task<IActionResult> RegisterExecutions([FromRoute] string consultNumber, [FromBody][Required] ExecutionsRequest executions)
    {
        await _consultUsecase.RegisterExecutionsAsync(consultNumber, executions);
        return Ok();
    }

    /// <summary>
    /// AP1009_検査結果入力情報を取得する
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
    /// AP1014_検査結果を登録する
    /// </summary>
    /// <param name="consultNumber">受診番号</param>
    /// <param name="results">検査結果登録項目</param>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost]
    [Route("api/v{version:apiVersion}/consult/{consultNumber}/results")]
    public async Task<IActionResult> RegisterResults([FromRoute][Required] string consultNumber, [FromBody] ResultsRequest results)
    {
        await _consultUsecase.RegisterResultsAsync(consultNumber, results);
        return Ok();
    }

    /// <summary>
    /// AP1013_検査結果を検証する
    /// </summary>
    /// <param name="consultNumber">受診番号</param>
    /// <param name="results">検査結果検証項目</param>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VerifyExamItems))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpPost]
    [Route("api/v{version:apiVersion}/consult/{consultNumber}/results/verify")]
    public async Task<IActionResult> VerifyResults([FromRoute][Required] string consultNumber, [FromBody] ResultsRequest results)
    {
        var response = await _consultUsecase.VerifyResults(consultNumber, results);
        return Ok(response);
    }

}
