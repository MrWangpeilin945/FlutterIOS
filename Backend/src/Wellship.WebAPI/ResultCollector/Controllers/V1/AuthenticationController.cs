using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// 認証コントローラー
/// </summary>
[ApiController]
[ApiVersion("1")]
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StaffLoginResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpPost]
    [Route("api/v{version:apiVersion}/staff/login")]
    public async Task<IActionResult> LoginAsync([FromBody] StaffLoginRequest request)
    {
        var accessToken = await _authenticationUsecase.LoginStaffAsync(request.LoginId, request.Password);
        var response = new StaffLoginResponse { Token = accessToken };
        return Ok(response);
    }
}
