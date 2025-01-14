using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// 職員コントローラー
/// </summary>
[ApiController]
[ApiVersion("1")]
public class StaffController : ControllerBase
{
    private readonly IStaffUsecase _administratorUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="staffUsecaseUsecase">職員ユースケース</param>
    public StaffController(IStaffUsecase staffUsecaseUsecase)
    {
        _administratorUsecase = staffUsecaseUsecase;
    }

    /// <summary>
    /// AP1002_職員の情報を取得する
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Staff))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/staff/profile")]
    public async Task<IActionResult> GetStaffAsync()
    {
        var result = await _administratorUsecase.GetStaffAsync();
        return Ok(result);
    }
}
