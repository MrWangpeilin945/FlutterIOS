namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 中止理由
/// </summary>
public class CancelReason
{
    /// <summary>
    /// 中止理由ID
    /// </summary>
    public required int CancelReasonId { get; init; }

    /// <summary>
    /// 名称
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 検査項目ID
    /// </summary>
    public required int ExamItemId { get; init; }
}
