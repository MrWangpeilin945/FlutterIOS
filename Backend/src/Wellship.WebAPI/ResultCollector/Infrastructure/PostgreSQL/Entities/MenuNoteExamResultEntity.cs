namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 検査メニュー特記_検査結果のエンティティ
/// </summary>
public class MenuNoteExamResultEntity
{
    /// <summary>
    /// 検査メニュー特記ID
    /// </summary>
    public int MenuNoteId { get; init; }

    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public required int ExamItemDetailId { get; init; }

    /// <summary>
    /// データソース種別
    /// </summary>
    public required int SourceType { get; init; }

    /// <summary>
    /// 表示順
    /// </summary>
    public required int OrderNumber { get; init; }
}
