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
    public ConsultController(IConsultUsecase consultUsecase)
    {
        _consultUsecase = consultUsecase;
    }

    /// <summary>
    /// 予約Noの存在を確認する
    /// </summary>
    /// <param name="reservationNoRequest">予約Noリクエスト</param>
    /// <returns>存在するか</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost]
    [Route("api/v{version:apiVersion}/consult/reservationNo/verify")]
    public IActionResult VerifyReservationNoAsync([FromBody] ReservationNoRequest reservationNoRequest)
    {
        _consultUsecase.VerifyReservationNo(reservationNoRequest);
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
}
