namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査結果出力履歴
/// </summary>
public class ExportHistory
{
    /// <summary>
    /// 出力履歴ID
    /// </summary>
    public required Guid ExportId { get; init; }

    /// <summary>
    /// 会場日程ID
    /// </summary>
    public required Guid PlaceScheduleId { get; init; }

    /// <summary>
    /// 出力件数
    /// </summary>
    public required int DataCount { get; init; }

    /// <summary>
    /// 出力日時
    /// </summary>
    public required DateTimeOffset ExportedAt { get; init; }

    /// <summary>
    /// 出力者
    /// </summary>
    public required string ExportedBy { get; init; }
}
