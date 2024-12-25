using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査実施判断ルール
/// </summary>
public class DecisionRule
{
    /// <summary>
    /// 検査実施判断ルールID
    /// </summary>
    public required int DecisionRuleId { get; init; }

    /// <summary>
    /// 検査実施判断ルール名
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 検査メニューID
    /// </summary>
    public required int ExamMenuId { get; init; }

    /// <summary>
    /// 優先度
    /// </summary>
    public required int Priority { get; init; }

    /// <summary>
    /// 発火条件種別
    /// </summary>
    public required RuleTriggerType TriggerType { get; init; }

    /// <summary>
    /// エラーレベル
    /// </summary>
    public required InputErrorLevel ErrorLevel { get; init; }

    /// <summary>
    /// 出力メッセージ
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// 判定値リスト
    /// </summary>
    public required IEnumerable<DecisionRuleEvaluation> Evaluations { get; init; }

    /// <summary>
    /// 検査項目明細リスト
    /// </summary>
    public required IEnumerable<DecisionRuleExamItemDetail> ExamItemDetails { get; init; }
}
