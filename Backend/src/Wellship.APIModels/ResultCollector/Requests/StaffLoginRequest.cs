using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Requests;

/// <summary>
/// 職員ログインのリクエストモデル
/// </summary>
public class StaffLoginRequest
{
    /// <summary>
    /// 職員ID
    /// </summary>
    [JsonPropertyName("staffId")]
    public int StaffId { get; set; }

    /// <summary>
    /// パスワード
    /// </summary>
    [JsonPropertyName("password")]
    public string Password { get; set; } = "";
}
