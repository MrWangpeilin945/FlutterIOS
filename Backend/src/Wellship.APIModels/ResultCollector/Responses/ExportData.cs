using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 連携対象検査結果
/// </summary>
public class ExportData
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ExportData(int placeScheduleId, string placeName, int placeScheduleLockingStatus, DateOnly examDate,
                      int dataCount, ExportDataDetail[] details)
    {
        PlaceScheduleId = placeScheduleId;
        PlaceName = placeName;
        PlaceScheduleLockingStatus = placeScheduleLockingStatus;
        ExamDate = examDate;
        DataCount = dataCount;
        Details = details;
    }

    /// <summary>
    /// 会場日程ID
    /// </summary>
    [JsonPropertyName("placeScheduleId")]
    public int PlaceScheduleId { get; }

    /// <summary>
    /// 会場名
    /// </summary>
    [JsonPropertyName("placeName")]
    public string PlaceName { get; }

    /// <summary>
    /// ロック状態
    /// </summary>
    [JsonPropertyName("placeScheduleLockingStatus")]
    public int PlaceScheduleLockingStatus { get; }

    /// <summary>
    /// 健診日
    /// </summary>
    [JsonPropertyName("examDate")]
    public DateOnly ExamDate { get; }

    /// <summary>
    /// 項目数
    /// </summary>
    [JsonPropertyName("dataCount")]
    public int DataCount { get; }

    /// <summary>
    /// 連携対象検査結果明細
    /// </summary>
    [JsonPropertyName("details")]
    public ExportDataDetail[] Details { get; }

}
