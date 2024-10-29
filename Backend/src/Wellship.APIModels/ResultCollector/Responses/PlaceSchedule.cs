using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 会場日程の班リスト
/// </summary>
public class PlaceSchedule
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PlaceSchedule(int placeScheduleId, int placeId, string placeName, string startTime)
    {
        PlaceScheduleId = placeScheduleId;
        PlaceId = placeId;
        PlaceName = placeName;
        StartTime = startTime;
    }

    /// <summary>
    /// 会場日程ID
    /// </summary>
    [JsonPropertyName("placeScheduleId")]
    public int PlaceScheduleId { get; set; }

    /// <summary>
    /// 会場ID
    /// </summary>
    [JsonPropertyName("placeId")]
    public int PlaceId { get; set; }

    /// <summary>
    /// 会場名
    /// </summary>
    [JsonPropertyName("placeName")]
    public string PlaceName { get; set; } = "";

    /// <summary>
    /// 開始時刻
    /// </summary>
    [JsonPropertyName("startTime")]
    public string StartTime { get; set; } = "";
}
