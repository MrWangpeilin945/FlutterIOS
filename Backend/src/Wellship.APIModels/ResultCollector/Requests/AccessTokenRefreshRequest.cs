using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Requests;

/// <summary>
/// アクセストークンをリフレッシュするためのリクエスト
/// </summary>
public class AccessTokenRefreshRequest
{
    /// <summary>
    /// アクセストークン
    /// </summary>
    [JsonPropertyName("token")]
    public required string Token { get; set; }
}