using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// 機器コントローラー
/// </summary>
[ApiController]
[ApiVersion("1")]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentUsecase _equipmentUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="equipmentUsecase">機器ユースケース</param>
    public EquipmentController(IEquipmentUsecase equipmentUsecase)
    {
        _equipmentUsecase = equipmentUsecase;
    }

    /// <summary>
    /// 機器連携設定を取得する
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Equipment))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/equipments")]
    public IActionResult GetEquipmentSettings([FromQuery] int examMenuId)
    {
        _equipmentUsecase.GetEquipmentSettings();
        return Ok();
    }
}
