using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Requests;

/// <summary>
/// 職員ログインのレスポンスモデル
/// </summary>
public class StaffLoginResponse
{
    /// <summary>
    /// トークン
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; set; } = "";
}
