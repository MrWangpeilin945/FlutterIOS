namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 会場日程
/// </summary>
public class PlaceSchedule
{
    /// <summary>
    /// 会場日程ID
    /// </summary>
    public int PlaceScheduleId { get; set; }

    /// <summary>
    /// 班ID
    /// </summary>
    public int TeamId { get; set; }

    /// <summary>
    /// 会場ID
    /// </summary>
    public int PlaceId { get; set; }

    /// <summary>
    /// 開始時刻
    /// </summary>
    public required string StartTime { get; set; }

    /// <summary>
    /// 終了時刻
    /// </summary>
    public required string EndTime { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateOnly Date { get; set; }
}
