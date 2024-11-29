using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査結果出力履歴
/// </summary>
public class ExportHistory
{
    /// <summary>
    /// 出力履歴ID
    /// </summary>
    [JsonPropertyName("exportId")]
    public required Guid ExportId { get; init; }

    /// <summary>
    /// 会場日程ID
    /// </summary>
    [JsonPropertyName("placeScheduleId")]
    public required int PlaceScheduleId { get; init; }

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
    /// 項目数
    /// </summary>
    [JsonPropertyName("dataCount")]
    public required int DataCount { get; init; }

    /// <summary>
    /// 出力日時
    /// </summary>
    [JsonPropertyName("exportedAt")]
    public required DateTime ExportedAt { get; init; }

    /// <summary>
    /// 出力者
    /// </summary>
    [JsonPropertyName("exportedBy")]
    public required string ExportedBy { get; init; }

}
