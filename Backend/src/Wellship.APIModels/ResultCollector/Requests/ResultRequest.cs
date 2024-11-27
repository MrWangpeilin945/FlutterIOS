using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Requests;

/// <summary>
/// 検査結果の登録の検査項目リクエストモデル
/// </summary>
public class ResultRequest
{
    /// <summary>
    /// 検査項目ID
    /// </summary>
    [JsonPropertyName("examItemId")]
    public required int ExamItemId { get; set; }

    /// <summary>
    /// 検査項目明細
    /// </summary>
    [JsonPropertyName("examItemDetails")]
    public required ExamItemDetailRequest[] ExamItemDetails { get; set; }

}