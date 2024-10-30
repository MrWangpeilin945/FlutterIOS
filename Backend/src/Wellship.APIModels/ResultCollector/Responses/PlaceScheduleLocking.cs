using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 会場ロック状態
/// </summary>
public class PlaceScheduleLocking
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PlaceScheduleLocking(int placeScheduleId, int placeId, string placeName, DateOnly examDate, 
                                int placeScheduleLockingStatus, DateTime updatedAt, string updatedBy)
    {
        PlaceScheduleId = placeScheduleId;
        PlaceId = placeId;
        PlaceName = placeName;
        ExamDate = examDate;
        PlaceScheduleLockingStatus = placeScheduleLockingStatus;
        UpdatedAt = updatedAt;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// 会場日程ID
    /// </summary>
    [JsonPropertyName("placeScheduleId")]
    public int PlaceScheduleId { get; }

    /// <summary>
    /// 会場ID
    /// </summary>
    [JsonPropertyName("placeId")]
    public int PlaceId { get; }

    /// <summary>
    /// 会場名
    /// </summary>
    [JsonPropertyName("placeName")]
    public string PlaceName { get; }

    /// <summary>
    /// 健診日
    /// </summary>
    [JsonPropertyName("examDate")]
    public DateOnly ExamDate { get; }

    /// <summary>
    /// ロック状態
    /// </summary>
    [JsonPropertyName("placeScheduleLockingStatus")]
    public int PlaceScheduleLockingStatus { get; }

    /// <summary>
    /// 最終更新日時
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; }

    /// <summary>
    /// 最終更新者
    /// </summary>
    [JsonPropertyName("updatedBy")]
    public string UpdatedBy { get; }

}
