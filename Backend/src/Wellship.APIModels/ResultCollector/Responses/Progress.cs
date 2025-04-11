using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 進捗状況
/// </summary>
public class Progress
{
    /// <summary>
    /// 検査メニューID
    /// </summary>
    [JsonPropertyName("examMenuId")]
    public required int ExamMenuId { get; init; }

    /// <summary>
    /// 検査メニュー名
    /// </summary>
    [JsonPropertyName("examMenuName")]
    public required string ExamMenuName { get; init; }

    /// <summary>
    /// 進捗明細
    /// </summary>
    [JsonPropertyName("details")]
    public required ProgressDetail[] Details { get; init; }
}
