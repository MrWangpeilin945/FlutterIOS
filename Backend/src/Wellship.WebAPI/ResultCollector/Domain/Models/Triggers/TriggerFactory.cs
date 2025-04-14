using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;

/// <summary>
/// トリガーのファクトリ
/// </summary>
public static class TriggerFactory
{
    /// <summary>
    /// 種別に応じてトリガーを生成します。
    /// </summary>
    /// <param name="triggerType">ルール発火条件種別</param>
    /// <param name="inputValues">入力値リスト</param>
    /// <param name="conditionValues">判定値リスト</param>
    /// <param name="errorLevel">エラーレベル</param>
    public static ITrigger CreateTrigger(RuleTriggerType triggerType, List<string> inputValues, List<string> conditionValues, InputErrorLevel errorLevel)
    {
        return triggerType switch
        {
            RuleTriggerType.ThresholdExceeded => new ThresholdExceeded(inputValues, conditionValues, errorLevel),
            RuleTriggerType.AllInputsNotEqual => new AllInputsNotEqual(inputValues, errorLevel),
            RuleTriggerType.NumericInputEqual => new NumericInputEqual(inputValues, errorLevel),
            RuleTriggerType.NumericInputNotEqual => new NumericInputNotEqual(inputValues, errorLevel),
            RuleTriggerType.NumericInputGreaterThan => new NumericInputGreaterThan(inputValues, errorLevel),
            RuleTriggerType.NumericInputGreaterThanOrEqual => new NumericInputGreaterThanOrEqual(inputValues, errorLevel),
            RuleTriggerType.NumericInputLessThan => new NumericInputLessThan(inputValues, errorLevel),
            RuleTriggerType.NumericInputLessThanOrEqual => new NumericInputLessThanOrEqual(inputValues, errorLevel),
            RuleTriggerType.NumericConditionEqual => new NumericConditionEqual(inputValues, conditionValues, errorLevel),
            RuleTriggerType.NumericConditionNotEqual => new NumericConditionNotEqual(inputValues, conditionValues, errorLevel),
            RuleTriggerType.NumericConditionGreaterThan => new NumericConditionGreaterThan(inputValues, conditionValues, errorLevel),
            RuleTriggerType.NumericConditionGreaterThanOrEqual => new NumericConditionGreaterThanOrEqual(inputValues, conditionValues, errorLevel),
            RuleTriggerType.NumericConditionLessThan => new NumericConditionLessThan(inputValues, conditionValues, errorLevel),
            RuleTriggerType.NumericConditionLessThanOrEqual => new NumericConditionLessThanOrEqual(inputValues, conditionValues, errorLevel),
            RuleTriggerType.AbsoluteGreaterThan => new AbsoluteGreaterThan(inputValues, conditionValues, errorLevel),
            RuleTriggerType.AbsoluteGreaterThanOrEqual => new AbsoluteGreaterThanOrEqual(inputValues, conditionValues, errorLevel),
            RuleTriggerType.AbsoluteLessThan => new AbsoluteLessThan(inputValues, conditionValues, errorLevel),
            RuleTriggerType.AbsoluteLessThanOrEqual => new AbsoluteLessThanOrEqual(inputValues, conditionValues, errorLevel),
            RuleTriggerType.StringIn => new StringIn(inputValues, conditionValues, errorLevel),
            RuleTriggerType.StringNotIn => new StringNotIn(inputValues, conditionValues, errorLevel),
            RuleTriggerType.AllStringsEqual => new AllStringsEqual(inputValues, errorLevel),
            RuleTriggerType.AnyStringNotEqual => new AnyStringNotEqual(inputValues, errorLevel),
            RuleTriggerType.StringEqualAndContainedInList => new StringEqualAndContainedInList(inputValues, conditionValues, errorLevel),
            _ => throw new NotImplementedException()
        };
    }
}
