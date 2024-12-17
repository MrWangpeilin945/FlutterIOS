namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 検査結果相関ルール_検査項目明細エンティティ
/// </summary>
public class CorrelationRuleExamItemDetailEntity
{
    /// <summary>
    /// 検査結果相関ルールID
    /// </summary>
    public int CorrelationRuleId { get; set; }

    /// <summary>
    /// 変数番号
    /// </summary>
    public int VariableNumber { get; set; }

    /// <summary>
    /// データソース種別
    /// </summary>
    public int SourceType { get; set; }

    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public int ExamItemDetailId { get; set; }
}
