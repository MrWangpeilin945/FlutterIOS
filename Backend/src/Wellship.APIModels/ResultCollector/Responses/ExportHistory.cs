using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査結果出力履歴
/// </summary>
public class ExportHistory
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ExportHistory(int placeScheduleId, string placeName, int placeScheduleLockingStatus, 
                           DateOnly examDate, int dataCount, DateTime exportedAt, string exportedBy)
    {
        PlaceScheduleId = placeScheduleId;
        PlaceName = placeName;
        PlaceScheduleLockingStatus = placeScheduleLockingStatus;
        ExamDate = examDate;
        DataCount = dataCount;
        ExportedAt = exportedAt;
        ExportedBy = exportedBy;
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
    /// 出力日時
    /// </summary>
    [JsonPropertyName("exportedAt")]
    public DateTime ExportedAt { get; }

    /// <summary>
    /// 出力者
    /// </summary>
    [JsonPropertyName("exportedBy")]
    public string ExportedBy { get; }

}
