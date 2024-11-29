namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 未受診の検査項目（受診単位）
/// 検査依頼（明細単位）に対して、検査結果または検査中止のレコードが存在すれば済とする。
/// </summary>
public class UnexaminedConsult
{
    /// <summary>
    /// 受診ID
    /// </summary>
    public required int ConsultId { get; init; }

    /// <summary>
    /// 受診番号
    /// </summary>
    public required string ConsultNumber { get; init; }

    /// <summary>
    /// 受診者ID
    /// </summary>
    public required int ExamineeId { get; init; }

    /// <summary>
    /// 未受診の検査項目リスト
    /// </summary>
    public required IEnumerable<UnexaminedExamMenu> UnexaminedExamMenus { get; init; }
}
