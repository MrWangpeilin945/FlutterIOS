using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// 健診メニューコントローラー
/// </summary>
[ApiController]
[ApiVersion("1.0")]
public class CheckupMenuController : ControllerBase
{
    private readonly ICheckupMenuUsecase _checkupMenuUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="checkupMenuUsecase">健診メニューユースケース</param>
    public CheckupMenuController(ICheckupMenuUsecase checkupMenuUsecase)
    {
        _checkupMenuUsecase = checkupMenuUsecase;
    }

    /// <summary>
    /// 健診メニュー一覧を取得する
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("api/v{version:apiVersion}/checkupMenus")]
    public IActionResult GetCheckupMenus()
    {
        _checkupMenuUsecase.GetCheckupMenus();
        return Ok();
    }
}
