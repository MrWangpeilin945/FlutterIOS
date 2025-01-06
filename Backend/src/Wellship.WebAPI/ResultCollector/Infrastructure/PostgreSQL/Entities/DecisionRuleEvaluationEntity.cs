namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 検査実施判断ルール_判定値エンティティ
/// </summary>
public class DecisionRuleEvaluationEntity
{
    /// <summary>
    /// 検査実施判断ルールID
    /// </summary>
    public int DecisionRuleId { get; set; }

    /// <summary>
    /// 変数番号
    /// </summary>
    public int VariableNumber { get; set; }

    /// <summary>
    /// 判定値
    /// </summary>
    public required string EvaluationValue { get; set; }
}
