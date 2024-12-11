using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 会場日程の班リスト
/// </summary>
public class PlaceSchedule
{
    /// <summary>
    /// 会場日程ID
    /// </summary>
    [JsonPropertyName("placeScheduleId")]
    public required Guid PlaceScheduleId { get; init; }

    /// <summary>
    /// 会場ID
    /// </summary>
    [JsonPropertyName("placeId")]
    public required Guid PlaceId { get; init; }

    /// <summary>
    /// 会場名
    /// </summary>
    [JsonPropertyName("placeName")]
    public required string PlaceName { get; init; }

    /// <summary>
    /// 開始時刻（HH:mm形式）
    /// </summary>
    [JsonPropertyName("startTime")]
    public required string StartTime { get; init; }
}
