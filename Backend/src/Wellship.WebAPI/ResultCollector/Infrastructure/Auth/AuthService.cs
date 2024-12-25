using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;

using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth.Settings;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth;

/// <summary>
/// 認証機能のサービスです
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 指定した職員のアクセストークンを生成します
    /// </summary>
    /// <param name="staff">職員</param>
    /// <returns></returns>
    public string GenerateAccessToken(Staff staff);
    /// <summary>
    /// 指定した職員のアクセストークンをリフレッシュします
    /// </summary>
    /// <param name="jwt">アクセストークン</param>
    /// <param name="refreshToken">リフレッシュトークン</param>
    /// <returns></returns>
    public ValueTask<(Staff staff, string refreshToken)> RefreshAccessTokenAsync(string jwt, string refreshToken);
}

/// <inheritdoc/>
/// <param name="authSettings">認証設定</param>
/// <param name="httpContextAccessor">アクセスされた情報を取得するためにHttpContextへのアクセスを使用します</param>
/// <param name="timeProvider"></param>
/// <param name="staffRepository">有効性確認のため職員情報を取得する際に使用します</param>
/// <param name="refreshTokenRepository"></param>
public class AuthService(AuthSettings authSettings,
                         IHttpContextAccessor httpContextAccessor,
                         TimeProvider timeProvider,
                         IStaffRepository staffRepository,
                         IRefreshTokenRepository refreshTokenRepository) : IAuthService
{
    private readonly AuthSettings _authSettings = authSettings;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly IStaffRepository _staffRepository = staffRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

    /// <inheritdoc/>
    public string GenerateAccessToken(Staff staff)
    {
        var host = _httpContextAccessor.HttpContext?.Request.Host.Value ?? "";
        var utcNow = _timeProvider.GetUtcNow();
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
            expires: utcNow.Add(_authSettings.AccessTokenLifetime).UtcDateTime,
            // 署名の設定
            signingCredentials: new SigningCredentials(_authSettings.JwtSigningKey, SecurityAlgorithms.HmacSha256)
        );
        // JWTの利用者
        token.Payload[JwtRegisteredClaimNames.Aud] = host is not null ? new string[] { host } : [];
        return tokenHandler.WriteToken(token);
    }

    /// <inheritdoc/>
    public async ValueTask<(Staff staff, string refreshToken)> RefreshAccessTokenAsync(string jwt, string refreshToken)
    {
        var (refreshable, staff) = await CanRefreshAccessTokenAsync(jwt, refreshToken);
        if (!refreshable || staff is null)
        {
            throw new WellshipAuthenticationException();
        }
        return (staff, GenerateAccessToken(staff));
    }

    /// <summary>
    /// アクセストークンをリフレッシュしてよいか確認します。
    /// </summary>
    /// <param name="jwt">アクセストークンとして使用しているjwt</param>
    /// <param name="refreshToken">リフレッシュトークン</param>
    /// <returns>バリデーション結果</returns>
    private async ValueTask<(bool result, Staff? staff)> CanRefreshAccessTokenAsync(string jwt, string refreshToken)
    {
        var host = _httpContextAccessor.HttpContext?.Request.Host.Value ?? "";
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = host,
            ValidateIssuer = true,
            ValidAudience = host,
            ValidateAudience = true,
            IssuerSigningKey = _authSettings.JwtSigningKey,
            ValidateIssuerSigningKey = true,

        };

        var tokenHandler = new JwtSecurityTokenHandler()
        {
            MapInboundClaims = false
        };
        var result = await tokenHandler.ValidateTokenAsync(jwt, tokenValidationParameters);
        if (result.SecurityToken is not JwtSecurityToken jst ||
            !jst.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            return (false, null);
        }

        // StaffIDが正しいことを確認します
        var sub = result.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (sub is null || !Guid.TryParse(sub, out var staffId))
        {
            return (false, null);
        }
        Staff? staff;
        try
        {
            staff = await _staffRepository.GetStaffByStaffIdAsync(staffId);
        }
        catch (StaffNotFoundException)
        {
            return (false, null);
        }
        // 職員が無効になっている場合は更新させません
        if (!staff.Enabled)
        {
            return (false, null);
        }
        // リフレッシュトークンをチェックします
        var storedRefreshToken = await _refreshTokenRepository.GetRefreshTokenOrNullAsync(staffId);
        if (storedRefreshToken is null || storedRefreshToken.Token != refreshToken || storedRefreshToken.ExpiresAt < _timeProvider.GetUtcNow())
        {
            return (false, null);
        }
        return (true, staff);
    }
}