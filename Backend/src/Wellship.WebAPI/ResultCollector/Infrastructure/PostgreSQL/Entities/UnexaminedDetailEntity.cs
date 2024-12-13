namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 未受診検査メニューのエンティティ
/// </summary>
public class UnexaminedMenuEntity
{
    /// <summary>
    /// 受診ID
    /// </summary>
    public required Guid ConsultId { get; init; }

    /// <summary>
    /// 受診番号
    /// </summary>
    public required string ConsultNumber { get; init; }

    /// <summary>
    /// 受診者ID
    /// </summary>
    public required Guid ExamineeId { get; init; }

    /// <summary>
    /// 検査メニューID
    /// </summary>
    public required int ExamMenuId { get; init; }

    /// <summary>
    /// 検査メニュー名
    /// </summary>
    public required string ExamMenuName { get; init; }
}
