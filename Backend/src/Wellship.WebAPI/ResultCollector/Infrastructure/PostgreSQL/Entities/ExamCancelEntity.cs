namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査中止のエンティティ
/// </summary>
public class ExamCancelEntity
{
    /// <summary>
    /// 受診ID
    /// </summary>
    public int ConsultId { get; set; }

    /// <summary>
    /// 検査項目ID
    /// </summary>
    public int ExamItemId { get; set; }

    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public int ExamItemDetailId { get; set; }

    /// <summary>
    /// 中止理由ID
    /// </summary>
    public int CancelReasonId { get; set; }
}
