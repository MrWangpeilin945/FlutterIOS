namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査メニュー単位の表示用検査結果
/// </summary>

public class DisplayExamResultMenu
{
    /// <summary>
    /// 検査メニューID
    /// </summary>
    public required int ExamMenuId { get; init; }

    /// <summary>
    /// 検査メニュー名
    /// </summary>
    public required string ExamMenuName { get; init; }

    /// <summary>
    /// 検査項目単位の表示用検査結果
    /// </summary>
    public required IEnumerable<DisplayExamResultItem> ExamItems { get; init; }
}