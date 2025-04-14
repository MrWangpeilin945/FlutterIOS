using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;

/// <summary>
/// 入力値の差の絶対値が判定値未満か
/// </summary>
public sealed class AbsoluteLessThan : TriggerBase
{
    private readonly List<string> _inputValues;
    private readonly List<string> _conditionValues;
    private readonly InputErrorLevel _inputErrorLevel;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public AbsoluteLessThan(List<string> inputValues, List<string> conditionValues, InputErrorLevel errorLevel)
    {
        _inputValues = inputValues;
        _conditionValues = conditionValues;
        _inputErrorLevel = errorLevel;
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

        return Math.Abs(input2 - input1) < threshold;
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
