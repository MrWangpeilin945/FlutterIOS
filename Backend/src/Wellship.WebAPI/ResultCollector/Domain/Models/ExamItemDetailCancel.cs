namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査項目明細単位の中止
/// </summary>
public class ExamItemDetailCancel
{
    /// <summary>
    /// 検査項目ID
    /// </summary>
    public required int ExamItemId { get; init; }

    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public required int ExamItemDetailId { get; init; }

    /// <summary>
    /// 中止理由ID
    /// </summary>
    public required int CancelReasonId { get; init; }
}
