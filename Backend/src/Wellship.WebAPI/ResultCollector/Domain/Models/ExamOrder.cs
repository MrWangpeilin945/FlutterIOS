namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査中止を受診で束ねたクラス
/// </summary>
public class ExamOrder
{
    /// <summary>
    /// 受診ID
    /// </summary>
    public required int ConsultId { get; init; }

    /// <summary>
    /// 検査項目明細単位の依頼
    /// </summary>
    public required IEnumerable<ExamItemDetailOrder> ExamItemDetailOrders { get; init; }
}
