using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 班ごとの会場日程リスト
/// </summary>
public class PlaceSchedulePlaces
{
    /// <summary>
    /// 班ID
    /// </summary>
    [JsonPropertyName("teamId")]
    public required int TeamId { get; init; }

    /// <summary>
    /// 班名
    /// </summary>
    [JsonPropertyName("teamName")]
    public required string TeamName { get; init; }

    /// <summary>
    /// 健診日
    /// </summary>
    [JsonPropertyName("examDate")]
    public required DateOnly ExamDate { get; init; }

    /// <summary>
    /// 会場日程リスト
    /// </summary>
    [JsonPropertyName("placeSchedules")]
    public required PlaceSchedule[] PlaceSchedules { get; init; }
}
