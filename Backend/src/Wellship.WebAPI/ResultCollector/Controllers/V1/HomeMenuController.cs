using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Asp.Versioning;

using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// ホームメニューコントローラー
/// </summary>
[ApiController]
[ApiVersion("1")]
[Authorize]
public class HomeMenuController : ControllerBase
{
    private readonly IHomeMenuUsecase _homeMenuUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="homeMenuUsecase">ホームメニューユースケース</param>
    public HomeMenuController(IHomeMenuUsecase homeMenuUsecase)
    {
        _homeMenuUsecase = homeMenuUsecase;
    }

    /// <summary>
    /// AP1005_ホームメニュー項目を取得する
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HomeMenuGroupList))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/homeMenus")]
    public async Task<IActionResult> GetHomeMenuSettingsAsync([FromQuery] Guid? placeScheduleId)
    {
        var results = await _homeMenuUsecase.GetHomeMenusAsync(placeScheduleId);
        return Ok(results);
    }
}
