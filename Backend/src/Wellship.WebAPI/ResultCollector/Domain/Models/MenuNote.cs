namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査メニュー特記マスタ
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
    /// 接尾辞
    /// </summary>
    public required string Suffix { get; init; }

    /// <summary>
    /// 検査メニュー特記と受診特記の関連
    /// </summary>
    public required IEnumerable<MenuNoteConsult> ConsultNotes { get; init; }

    /// <summary>
    /// 検査メニュー特記と検査結果（明細単位）の関連
    /// </summary>
    public required IEnumerable<MenuNoteExamResult> ExamResults { get; init; }
}
