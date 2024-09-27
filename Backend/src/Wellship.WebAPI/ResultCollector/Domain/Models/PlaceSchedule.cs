namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 会場日程
/// </summary>
public class PlaceSchedule
{

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PlaceSchedule(int id, Place place, Team team, DateOnly date, string startTime, string endTime)
    {
        Id = id;
        Place = place;
        Team = team;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
    }

    /// <summary>
    /// 会場日程ID
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// 会場
    /// </summary>
    public Place Place { get; }

    /// <summary>
    /// 班
    /// </summary>
    public Team Team { get; }

    /// <summary>
    /// 健診日
    /// </summary>
    public DateOnly Date { get; }

    /// <summary>
    /// 開始時刻
    /// </summary>
    public string StartTime { get; }

    /// <summary>
    /// 終了時刻
    /// </summary>
    public string EndTime { get; }
}
