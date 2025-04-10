using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Requests;

/// <summary>
/// 検査結果を取り消すリクエストモデル
/// </summary>
public class ResultDeleteRequest
{
    /// <summary>
    /// 削除対象の検査明細項目ID
    /// </summary>
    [JsonPropertyName("examItemDetailIds")]
    public required int[] ExamItemDetailIds { get; set; }
}
