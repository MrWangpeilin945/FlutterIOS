namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 検査メニュー特記のエンティティ
/// </summary>
public class MenuNoteEntity
{
    /// <summary>
    /// 検査メニュー特記ID
    /// </summary>
    public required int MenuNoteId { get; init; }

    /// <summary>
    /// 名称
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 表示順
    /// </summary>
    public required int OrderNumber { get; init; }

    /// <summary>
    /// 検査メニューID
    /// </summary>
    public required int ExamMenuId { get; init; }

    /// <summary>
    /// 接尾辞
    /// </summary>
    public required string? Suffix { get; init; }
}
