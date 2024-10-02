using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// 認証コントローラー
/// </summary>
[ApiController]
[ApiVersion("1.0")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationUsecase _authenticationUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="authenticationUsecase">認証ユースケース</param>
    public AuthenticationController(IAuthenticationUsecase authenticationUsecase)
    {
        _authenticationUsecase = authenticationUsecase;
    }

    /// <summary>
    /// ログインする
    /// </summary>
    [HttpPost]
    [Route("api/v{version:apiVersion}/login")]
    public IActionResult Login()
    {
        _authenticationUsecase.Login();
        return Ok();
    }    
}
