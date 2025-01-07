using System.Security.Cryptography;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// リフレッシュトークン情報
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// トークン文字列
    /// </summary>
    public required string Token { get; init; }
    /// <summary>
    /// トークンの有効期限
    /// </summary>
    public required DateTimeOffset ExpiresAt { get; init; }

    /// <summary>
    /// トークンを生成します。
    /// </summary>
    /// <param name="expiresAt">トークンの有効期限</param>
    public static RefreshToken Create(DateTimeOffset expiresAt) => new()
    {
        Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
        ExpiresAt = expiresAt
    };
}