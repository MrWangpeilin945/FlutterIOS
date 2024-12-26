using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth.Settings;

/// <summary>
/// 認証関連の設定
/// </summary>
public class AuthSettings
{
    /// <summary>
    /// アクセストークンの寿命
    /// </summary>
    public required TimeSpan AccessTokenLifetime { get; init; }
    /// <summary>
    /// リフレッシュトークンの寿命
    /// </summary>
    public required TimeSpan RefreshTokenLifeTime { get; init; }
    /// <summary>
    /// JWT署名鍵（文字列）
    /// </summary>
    public required string SecretKey { private get; init; }

    // NOTE: JWTに共通鍵を使用する前提で書いています。

    /// <summary>
    /// JWT署名鍵（Binary）
    /// </summary>
    public SecurityKey JwtSigningKey => _jwtSigningKey ??= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));

    private SecurityKey? _jwtSigningKey;
}