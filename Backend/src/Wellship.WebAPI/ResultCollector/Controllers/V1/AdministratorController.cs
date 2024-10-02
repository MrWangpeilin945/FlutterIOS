using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// 管理者コントローラー
/// </summary>
[ApiController]
[ApiVersion("1.0")]
public class AdministratorController : ControllerBase
{
    private readonly IAdministratorUsecase _administratorUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="administratorUsecase">管理者ユースケース</param>
    public AdministratorController(IAdministratorUsecase administratorUsecase)
    {
        _administratorUsecase = administratorUsecase;
    }

    /// <summary>
    /// 管理者の情報を取得する
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("api/v{version:apiVersion}/administrators/{administratorId}")]
    public IActionResult GetAdministrator()
    {
        _administratorUsecase.GetAdministrator();
        return Ok();
    }    

}
