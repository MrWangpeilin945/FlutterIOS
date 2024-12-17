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
        if (_inputValues.Count != 2 || _conditionValues.Count != 1)
        {
            return false;
        }
        if (!decimal.TryParse(_inputValues[0], out var input1))
        {
            return false;
        }
        if (!decimal.TryParse(_inputValues[1], out var input2))
        {
            return false;
        }
        if (!decimal.TryParse(_conditionValues[0], out var threshold))
        {
            return false;
        }

        var absoluteDifference = Math.Abs(input2 - input1);
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
