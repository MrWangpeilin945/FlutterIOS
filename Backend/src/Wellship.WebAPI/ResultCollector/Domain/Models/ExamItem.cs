namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査項目
/// </summary>
public class ExamItem
{
    /// <summary>
    /// 配置番号
    /// </summary>
    public required int PositionNumber { get; init; }

    /// <summary>
    /// 検査項目ID
    /// </summary>
    public required int ExamItemId { get; init; }

    /// <summary>
    /// 検査項目名
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 検査項目明細
    /// </summary>
    public required IEnumerable<ExamItemDetail> ExamItemDetails { get; init; }

}