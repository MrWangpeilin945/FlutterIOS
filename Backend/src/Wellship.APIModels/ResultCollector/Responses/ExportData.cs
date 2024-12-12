using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 連携対象検査結果
/// </summary>
public class ExportData
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
    /// ロック状態
    /// </summary>
    [JsonPropertyName("placeScheduleLockingStatus")]
    public required int PlaceScheduleLockingStatus { get; init; }

    /// <summary>
    /// 健診日
    /// </summary>
    [JsonPropertyName("examDate")]
    public required DateOnly ExamDate { get; init; }

    /// <summary>
    /// 開始時刻
    /// </summary>
    public required string StartTime { get; init; }

    /// <summary>
    /// 連携対象検査結果明細
    /// </summary>
    [JsonPropertyName("details")]
    public required ExportDataDetail[] Details { get; init; }
}
