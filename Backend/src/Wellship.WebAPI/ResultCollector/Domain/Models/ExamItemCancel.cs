namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査項目単位の中止
/// 今のところ検査中止を検査項目単位で登録するために使用する
/// </summary>
public class ExamItemCancel
{
    /// <summary>
    /// 検査項目ID
    /// </summary>
    public required int ExamItemId { get; init; }

    /// <summary>
    /// 中止理由ID
    /// </summary>
    public required int CancelReasonId { get; init; }
}
