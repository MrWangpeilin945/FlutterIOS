namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査メニュー特記
/// 表示用の設定
/// </summary>
public class MenuNote
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
    /// 検査メニューID
    /// </summary>
    public required int ExamMenuId { get; init; }

    /// <summary>
    /// 検査メニュー特記と検査項目特記の関連
    /// </summary>
    public required IEnumerable<MenuNoteExamItem> ExamItemNotes { get; init; }

    /// <summary>
    /// 検査メニュー特記と検査結果（明細単位）の関連
    /// </summary>
    public required IEnumerable<MenuNoteExamResult> ExamResults { get; init; }
}
