namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 未受診の検査メニュー
/// </summary>
public class UnexaminedExamMenu
{
    /// <summary>
    /// 検査メニューID
    /// </summary>
    public required int ExamMenuId { get; init; }

    /// <summary>
    /// 検査メニュー名
    /// </summary>
    public required string ExamMenuName { get; init; }
}
