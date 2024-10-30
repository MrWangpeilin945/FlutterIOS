using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Requests;

/// <summary>
/// 検査結果連携状態リクエストモデル
/// </summary>
public class IntegrationStatusRequest
{
    /// <summary>
    /// 受診番号
    /// </summary>
    [JsonPropertyName("consultNumber")]
    public string ConsultNumber { get; set; } = "";

    /// <summary>
    /// 連携状態
    /// </summary>
    [JsonPropertyName("integrationStatus")]
    public int IntegrationStatus{ get; set; }
}
