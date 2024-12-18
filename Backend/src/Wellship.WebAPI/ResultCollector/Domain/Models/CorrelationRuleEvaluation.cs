namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査結果相関ルール_判定値
/// </summary>
public class CorrelationRuleEvaluation
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
