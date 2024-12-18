namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 検査結果相関ルール_判定値エンティティ
/// </summary>
public class CorrelationRuleEvaluationEntity
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
    /// 判定値
    /// </summary>
    public required string EvaluationValue { get; set; }
}
