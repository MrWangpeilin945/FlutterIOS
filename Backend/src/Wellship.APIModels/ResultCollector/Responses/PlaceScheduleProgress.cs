using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 会場日程の進捗状況
/// </summary>
public class PlaceScheduleProgress
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PlaceScheduleProgress(Guid placeScheduleId, string placeName, DateOnly examDate, Progress[] progress)
    {
        PlaceScheduleId = placeScheduleId;
        PlaceName = placeName;
        ExamDate = examDate;
        Progress = progress;
    }

    /// <summary>
    /// 会場日程ID
    /// </summary>
    [JsonPropertyName("placeScheduleId")]
    public Guid PlaceScheduleId { get; }

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
    /// 進捗状況
    /// </summary>
    [JsonPropertyName("progress")]
    public Progress[] Progress { get; }

}
