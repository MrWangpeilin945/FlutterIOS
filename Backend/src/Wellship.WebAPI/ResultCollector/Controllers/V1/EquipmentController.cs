using System.ComponentModel.DataAnnotations;

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
    /// 検査機器一覧を取得する
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EquipmentList))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/equipments")]
    public async Task<IActionResult> GetEquipmentSettingsAsync([FromQuery][Required] int examMenuId)
    {
        var results = await _equipmentUsecase.GetEquipmentsAsync(examMenuId);
        return Ok(results);
    }
}
