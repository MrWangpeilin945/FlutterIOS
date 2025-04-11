using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 受診者情報リスト
/// </summary>
public class ConsultExamineeList
{
    /// <summary>
    /// 会場日程ID
    /// </summary>
    [JsonPropertyName("placeScheduleId")]
    public required Guid PlaceScheduleId { get; init; }

    /// <summary>
    /// 会場名
    /// </summary>
    [JsonPropertyName("placeName")]
    public required string PlaceName { get; init; }

    /// <summary>
    /// 健診日
    /// </summary>
    [JsonPropertyName("examDate")]
    public required DateOnly ExamDate { get; init; }

    /// <summary>
    /// 進捗状況
    /// </summary>
    [JsonPropertyName("progress")]
    public required Progress Progress { get; init; }

    /// <summary>
    /// 受診者情報リスト
    /// </summary>
    [JsonPropertyName("examinees")]
    public required ConsultExaminee[] Examinees { get; init; }
}