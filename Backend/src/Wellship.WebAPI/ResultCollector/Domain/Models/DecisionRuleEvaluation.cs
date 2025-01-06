namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査実施判断ルール_判定値
/// </summary>
public class DecisionRuleEvaluation
{
    /// <summary>
    /// 変数番号
    /// </summary>
    public required int VariableNumber { get; init; }

    /// <summary>
    /// 判定値
    /// </summary>
    public required string EvaluationValue { get; init; }
}
