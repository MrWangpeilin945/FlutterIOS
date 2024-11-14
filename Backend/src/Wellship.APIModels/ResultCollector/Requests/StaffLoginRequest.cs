using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Requests;

/// <summary>
/// 職員ログインのリクエストモデル
/// </summary>
public class StaffLoginRequest
{
    /// <summary>
    /// 職員のログインID
    /// </summary>
    [JsonPropertyName("loginId")]
    public string LoginId { get; set; } = null!;

    /// <summary>
    /// パスワード
    /// </summary>
    [JsonPropertyName("password")]
    public string Password { get; set; } = null!;
}
