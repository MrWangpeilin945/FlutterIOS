namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 集計検査進捗
/// 会場日程単位でまとめる。
/// </summary>
public class AggregatedProgress
{
    /// <summary>
    /// 会場日程ID
    /// </summary>
    public required Guid PlaceScheduleId { get; init; }

    /// <summary>
    /// 検査項目ごとの進捗状況
    /// </summary>
    public required IEnumerable<AggregatedProgressDetail> AggregatedProgressDetails { get; init; }
}
