namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 未受診の検査項目明細
/// 検査依頼（明細単位）に対して、検査結果または検査中止のレコードが存在すれば済とする。
/// </summary>
public class UnexaminedDetail
{
    /// <summary>
    /// 受診ID
    /// </summary>
    public required int ConsultId { get; init; }

    /// <summary>
    /// 未受診の検査項目明細IDリスト
    /// </summary>
    public required IEnumerable<int> ExamItemDetailIds { get; init; }
}
