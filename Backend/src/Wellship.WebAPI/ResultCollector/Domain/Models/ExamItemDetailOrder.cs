namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査項目明細単位の依頼
/// </summary>
public class ExamItemDetailOrder
{
    /// <summary>
    /// 検査項目ID
    /// </summary>
    public required int ExamItemId { get; init; }

    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public required int ExamItemDetailId { get; init; }

}
