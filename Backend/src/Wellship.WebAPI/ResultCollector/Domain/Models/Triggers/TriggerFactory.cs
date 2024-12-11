using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;

/// <summary>
/// トリガーのファクトリ
/// </summary>
public class TriggerFactory
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
            _ => throw new NotImplementedException()
        };
    }
}
