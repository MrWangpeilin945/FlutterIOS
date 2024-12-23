using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 関連検査項目
/// </summary>
public class RelatedExamItem
{
    /// <summary>
    /// 検査項目名
    /// </summary>
    [JsonPropertyName("examItemName")]
    public required string ExamItemName { get; init; }

    /// <summary>
    /// 検査結果
    /// </summary>
    [JsonPropertyName("examResult")]
    public required string ExamResult { get; init; }

}
