namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 会場日程と会場と班をJoinしたエンティティ
/// </summary>
public class PlaceSchedulePlaceTeam
{
    /// <summary>
    /// 会場日程ID
    /// </summary>
    public required Guid PlaceScheduleId { get; set; }

    /// <summary>
    /// 状況
    /// </summary>
    public required int Status { get; set; }

    /// <summary>
    /// 健診日
    /// </summary>
    public required DateOnly ExamDate { get; set; }

    /// <summary>
    /// 開始時刻
    /// </summary>
    public required string StartTime { get; set; }

    /// <summary>
    /// 会場ID
    /// </summary>
    public required Guid PlaceId { get; set; }

    /// <summary>
    /// 会場コード
    /// </summary>
    public required string PlaceCode { get; set; }

    /// <summary>
    /// 会場名
    /// </summary>
    public required string PlaceName { get; set; }

    /// <summary>
    /// 会場表示順
    /// </summary>
    public required int PlaceOrderNumber { get; set; }

    /// <summary>
    /// 班ID
    /// </summary>
    public required Guid TeamId { get; set; }

    /// <summary>
    /// 班コード
    /// </summary>
    public required string TeamCode { get; set; }

    /// <summary>
    /// 班名
    /// </summary>
    public required string TeamName { get; set; }

    /// <summary>
    /// 班表示順
    /// </summary>
    public required int TeamOrderNumber { get; set; }
}
