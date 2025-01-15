using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 会場ロック状態
/// </summary>
public class PlaceScheduleStatus
{
    /// <summary>
    /// 会場日程ID
    /// </summary>
    public required Guid PlaceScheduleId { get; init; }

    /// <summary>
    /// 会場ID
    /// </summary>
    public required Guid PlaceId { get; init; }

    /// <summary>
    /// 会場名
    /// </summary>
    public required string PlaceName { get; init; }

    /// <summary>
    /// 健診日
    /// </summary>
    public required DateOnly ExamDate { get; init; }

    /// <summary>
    /// 状況
    /// </summary>
    public required PlaceScheduleLockingStatus Status { get; init; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// 作成者
    /// </summary>
    public required string CreatedBy { get; init; }

}
