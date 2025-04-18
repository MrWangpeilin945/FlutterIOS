namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査項目単位の表示用検査結果
/// </summary>

public class DisplayExamResultItem
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
    /// 検査項目明細単位の表示用検査結果
    /// </summary>
    public required IEnumerable<DisplayExamResultItemDetail> ExamItemDetails { get; init; }
}