namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 未受診の検査項目
/// </summary>
public class UnexaminedExamItem
{
    /// <summary>
    /// 検査項目ID
    /// </summary>
    public required int ExamItemId { get; init; }

    /// <summary>
    /// 検査項目名
    /// </summary>
    public required string ExamItemName { get; init; }

    /// <summary>
    /// 未受診の検査項目明細IDリスト
    /// </summary>
    public required IEnumerable<int> ExamItemDetailIds { get; init; }
}
