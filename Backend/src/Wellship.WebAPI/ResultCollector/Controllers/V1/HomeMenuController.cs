using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// ホームメニューコントローラー
/// </summary>
[ApiController]
[ApiVersion("1")]
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
    /// ホームメニュー項目を取得する
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("api/v{version:apiVersion}/homeMenus")]
    public IActionResult GetHomeMenuSettings()
    {
        _homeMenuUsecase.GetHomeMenus();
        return Ok();
    }
}
