using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;

/// <summary>
/// 2つの数値の差分がしきい値を超えるか
/// </summary>
public sealed class ThresholdExceeded : TriggerBase
{
    private readonly List<string> _inputValues;
    private readonly List<string> _conditionValues;
    private readonly InputErrorLevel _inputErrorLevel;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ThresholdExceeded(List<string> inputValues, List<string> conditionValues, InputErrorLevel errorLevel)
    {
        _inputValues = inputValues;
        _inputErrorLevel = errorLevel;
        _conditionValues = conditionValues;
    }

    /// <summary>
    /// トリガーの条件を満たすか計算します。
    /// </summary>
    public override bool IsMatch()
    {
        decimal input1 = Convert.ToDecimal(_inputValues[0]);
        decimal input2 = Convert.ToDecimal(_inputValues[1]);
        decimal threshold = Convert.ToDecimal(_conditionValues[0]);

        decimal absoluteDifference = Math.Abs(input2 - input1);

        return absoluteDifference > threshold;
    }

    /// <summary>
    /// 入力値を検証して入力エラーレベルを取得します。
    /// </summary>
    public override InputErrorLevel GetErrorLevel()
    {
        if (IsMatch())
        {
            return _inputErrorLevel;
        }
        return InputErrorLevel.正常;
    }
}
