using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査項目明細単位の表示用検査結果
/// </summary>
public class DisplayExamResultItemDetail
{
    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    [JsonPropertyName("examItemDetailId")]
    public required int ExamItemDetailId { get; init; }

    /// <summary>
    /// 検査項目明細名
    /// </summary>
    [JsonPropertyName("examItemDetailName")]
    public required string ExamItemDetailName { get; init; }

    /// <summary>
    /// 今回値
    /// </summary>
    [JsonPropertyName("currentResult")]
    public required string CurrentResult { get; init; }

    /// <summary>
    /// 過去値
    /// </summary>
    [JsonPropertyName("pastResult")]
    public required string PastResult { get; init; }

    /// <summary>
    /// 過去検査日
    /// </summary>
    [JsonPropertyName("pastDate")]
    public DateOnly? PastDate { get; init; }

    /// <summary>
    /// 直近判定
    /// </summary>
    [JsonPropertyName("isRecent")]
    public required bool IsRecent { get; init; }
}