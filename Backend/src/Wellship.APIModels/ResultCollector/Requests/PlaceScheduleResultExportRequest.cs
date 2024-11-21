using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Requests;

/// <summary>
/// 会場日程データ出力状況のリクエストモデル
/// </summary>
public class PlaceScheduleResultExportRequest
{
    /// <summary>
    /// 会場日程ID
    /// </summary>
    [JsonPropertyName("placeScheduleId")]
    public int PlaceScheduleId { get; set; }

    /// <summary>
    /// 会場日程データ出力状況
    /// </summary>
    [JsonPropertyName("placeScheduleResultExportStatus")]
    public int PlaceScheduleResultExportStatus { get; set; }
}
