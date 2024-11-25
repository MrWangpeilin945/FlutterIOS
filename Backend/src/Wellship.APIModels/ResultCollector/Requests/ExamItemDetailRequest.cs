using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Requests;

/// <summary>
/// 検査結果の登録の検査項目明細リクエストモデル
/// </summary>
public class ExamItemDetailRequest
{
    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    [JsonPropertyName("examItemDetailId")]
    public int ExamItemDetailId { get; set; }

    /// <summary>
    /// 値
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; } = "";
}