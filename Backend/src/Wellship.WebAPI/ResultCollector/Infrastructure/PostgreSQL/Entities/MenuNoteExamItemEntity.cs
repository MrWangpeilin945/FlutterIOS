namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 検査メニュー特記_検査項目特記のエンティティ
/// </summary>
public class MenuNoteExamItemEntity
{
    /// <summary>
    /// 検査メニュー特記ID
    /// </summary>
    public int MenuNoteId { get; init; }

    /// <summary>
    /// 検査項目ID
    /// </summary>
    public required int ExamItemId { get; init; }

    /// <summary>
    /// 表示順
    /// </summary>
    public required int OrderNumber { get; init; }
}
