using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth.Settings;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth;

/// <summary>
/// 認証機能のサービスです
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// セッションJWTを生成します
    /// </summary>
    /// <param name="staff">ユーザー</param>
    /// <returns></returns>
    public string GenerateAccessToken(Staff staff);
}

/// <inheritdoc/>
/// <param name="authSettings">認証設定</param>
/// <param name="httpContextAccessor">アクセスされた情報を取得するためにHttpContextへのアクセスを使用します</param>
public class AuthService(AuthSettings authSettings, IHttpContextAccessor httpContextAccessor) : IAuthService
{
    private readonly AuthSettings _authSettings = authSettings;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    /// <inheritdoc/>
    public string GenerateAccessToken(Staff staff)
    {
        var host = _httpContextAccessor.HttpContext?.Request.Host.Value ?? "";
        var utcNow = TimeProvider.System.GetUtcNow();
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = new JwtSecurityToken(
            // JWTの発行者
            issuer: host,
            claims: [
                // JWTによる認証の対象となるユーザーの識別子
                new(JwtRegisteredClaimNames.Sub, staff.StaffId.ToString()),
                // 発行時点のサーバー時刻(UnixTime)
                new(JwtRegisteredClaimNames.Iat, utcNow.ToUnixTimeSeconds().ToString()),
                // 認証したユーザーのロール
                new(CustomClaimTypes.Role, staff.Role.ToString()),
            ],
            // JWTが有効になる時刻
            notBefore: utcNow.UtcDateTime,
            // JWTが失効する時刻
            expires: utcNow.Add(_authSettings.Lifetime).UtcDateTime,
            // 署名の設定
            signingCredentials: new SigningCredentials(_authSettings.JwtSigningKey, SecurityAlgorithms.HmacSha256)
        );
        // JWTの利用者
        token.Payload[JwtRegisteredClaimNames.Aud] = host is not null ? new string[] { host } : [];
        return tokenHandler.WriteToken(token);
    }
}