using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査メニュー単位の表示用検査結果
/// </summary>
public class DisplayExamResultMenu
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
    /// 検査項目単位の表示用検査結果
    /// </summary>
    [JsonPropertyName("examItems")]
    public required IEnumerable<DisplayExamResultItem> ExamItems { get; init; }
}