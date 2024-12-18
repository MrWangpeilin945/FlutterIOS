using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 進捗状況
/// </summary>
public class Progress
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
    /// 進捗明細
    /// </summary>
    [JsonPropertyName("details")]
    public required ProgressDetail[] Details { get; init; }
}
