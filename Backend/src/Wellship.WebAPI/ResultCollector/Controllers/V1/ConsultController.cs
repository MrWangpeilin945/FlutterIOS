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
[ApiVersion("1.0")]
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
    public IActionResult VerifyConsultNumberAsync([FromBody] ConsultNumberRequest consultNumberRequest)
    {
        _consultUsecase.VerifyConsultNumber(consultNumberRequest);
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
    public IActionResult GetUnexaminedMenus([FromRoute][Required] string consultNumber)
    {
        return Ok();
    }

    /// <summary>
    /// 未受診の健診メニューを取得する
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("api/v{version:apiVersion}/consult/{consultId}/unexaminedMenus")]
    public IActionResult GetUnexaminedMenus()
    {
        _consultUsecase.GetUnexaminedMenus();
        return Ok();
    }

    /// <summary>
    /// 簡易な受診者情報を取得する
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("api/v{version:apiVersion}/consult/{consultId}/simple")]
    public IActionResult GetSimpleExaminee()
    {
        _consultUsecase.GetSimpleExaminee();
        return Ok();
    }

    /// <summary>
    /// 詳細な受診者情報を取得する
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("api/v{version:apiVersion}/consult/{consultId}/detail")]
    public IActionResult GetDetailedExaminee()
    {
        _consultUsecase.GetDetailedExaminee();
        return Ok();
    }

    /// <summary>
    /// 検査結果の連携状態を変更する
    /// </summary>
    /// <returns></returns>
    [HttpPut]
    [Route("api/v{version:apiVersion}/consult/{consultId}/integrationStatus")]
    public IActionResult ChangeIntegrationStatus()
    {
        _consultUsecase.ChangeIntegrationStatus();
        return Ok();
    }
}
