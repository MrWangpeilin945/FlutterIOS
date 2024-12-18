using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 会場日程
/// </summary>
public class PlaceSchedule
{
    /// <summary>
    /// 会場日程ID
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// 会場
    /// </summary>
    public required Place Place { get; init; }

    /// <summary>
    /// 班
    /// </summary>
    public required Team Team { get; init; }

    /// <summary>
    /// 健診日
    /// </summary>
    public required DateOnly ExamDate { get; init; }

    /// <summary>
    /// 開始時刻（HHmm形式）
    /// </summary>
    public required string StartTime { get; init; }

    /// <summary>
    /// 会場ロック状況
    /// </summary>
    public required PlaceScheduleLockingStatus PlaceScheduleLockingStatus { get; init; }

    /// <summary>
    /// 開始時刻をHH:mm形式で取得します。
    /// </summary>
    public string FormatStartTimeString
    {
        get => $"{StartTime.Substring(0, 2)}:{StartTime.Substring(2, 2)}";
    }
}
