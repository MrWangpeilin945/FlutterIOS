using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査項目単位の表示用検査結果
/// </summary>
public class DisplayExamResultItem
{
    /// <summary>
    /// 検査項目ID
    /// </summary>
    [JsonPropertyName("examItemId")]
    public required int ExamItemId { get; init; }

    /// <summary>
    /// 検査項目名
    /// </summary>
    [JsonPropertyName("examItemName")]
    public required string ExamItemName { get; init; }

    /// <summary>
    /// 検査項目明細単位の表示用検査結果
    /// </summary>
    [JsonPropertyName("examItemDetails")]
    public required IEnumerable<DisplayExamResultItemDetail> ExamItemDetails { get; init; }
}