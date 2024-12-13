using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 会場ロック状態
/// </summary>
public class PlaceScheduleLocking
{
    /// <summary>
    /// 会場日程ID
    /// </summary>
    [JsonPropertyName("placeScheduleId")]
    public Guid PlaceScheduleId { get; init;}

    /// <summary>
    /// 会場ID
    /// </summary>
    [JsonPropertyName("placeId")]
    public Guid PlaceId { get; init;}

    /// <summary>
    /// 会場名
    /// </summary>
    [JsonPropertyName("placeName")]
    public string PlaceName { get; init;} = "";

    /// <summary>
    /// 健診日
    /// </summary>
    [JsonPropertyName("examDate")]
    public DateOnly ExamDate { get; init;}

    /// <summary>
    /// ロック状態
    /// </summary>
    [JsonPropertyName("placeScheduleLockingStatus")]
    public int PlaceScheduleLockingStatus { get; init;}

    /// <summary>
    /// 最終更新日時
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; init;}

    /// <summary>
    /// 最終更新者
    /// </summary>
    [JsonPropertyName("updatedBy")]
    public string UpdatedBy { get; init;} = "";

}
