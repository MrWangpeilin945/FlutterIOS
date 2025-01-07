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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost]
    [Route("api/v{version:apiVersion}/staff/login")]
    public async ValueTask<IActionResult> LoginAsync([FromBody] StaffLoginRequest request)
    {
        var (accessToken, refreshToken) = await _authenticationUsecase.LoginStaffAsync(request.LoginId, request.Password);
        var response = new StaffLoginResponse { Token = accessToken };

        var path = HttpContext.Request.Path + "/refresh";
        var cookieOptions = new CookieOptions()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = path
        };
        HttpContext.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        return Ok(response);
    }

    /// <summary>
    /// アクセストークンをリフレッシュする
    /// </summary>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StaffLoginResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost]
    [Route("api/v{version:apiVersion}/staff/login/refresh")]
    public async ValueTask<IActionResult> RefreshAsync([FromBody] AccessTokenRefreshRequest request)
    {
        var refreshToken = Request.Cookies.SingleOrDefault(x => x.Key == "refreshToken");
        var (newAccessToken, newRefreshToken) = await _authenticationUsecase.RefreshAccessTokenAsync(request.Token, refreshToken.Value);
        var response = new StaffLoginResponse { Token = newAccessToken };

        var path = HttpContext.Request.Path;
        var cookieOptions = new CookieOptions()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = path
        };
        HttpContext.Response.Cookies.Append("refreshToken", newRefreshToken, cookieOptions);
        return Ok(response);
    }
}
