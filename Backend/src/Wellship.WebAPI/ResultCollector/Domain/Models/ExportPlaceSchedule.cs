namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 受診の結果出力状況を会場日程ごとに集約したモデル
/// </summary>
public class ExportPlaceSchedule
{
    /// <summary>
    /// 会場日程ID
    /// </summary>
    public required int PlaceScheduleId { get; init; }

    /// <summary>
    /// 未出力
    /// </summary>
    public required int ExportStatusCount11 { get; init; }

    /// <summary>
    /// 出力保留
    /// </summary>
    public required int ExportStatusCount21 { get; init; }

    /// <summary>
    /// 出力済み
    /// </summary>
    public required int ExportStatusCount31 { get; init; }
}
