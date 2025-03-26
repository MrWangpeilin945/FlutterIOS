using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;

/// <summary>
/// 入力1が判定1以下か
/// </summary>
public sealed class NumericConditionLessThanOrEqual : TriggerBase
{
    private readonly List<string> _inputValues;
    private readonly InputErrorLevel _inputErrorLevel;
    private readonly List<string> _conditionValues;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public NumericConditionLessThanOrEqual(List<string> inputValues, List<string> conditionValues, InputErrorLevel errorLevel)
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
        if (_inputValues.Count != 1 || _conditionValues.Count != 1)
        {
            return false;
        }

        if (!decimal.TryParse(_inputValues[0], out var input1))
        {
            return false;
        }
        if (!decimal.TryParse(_conditionValues[0], out var condition1))
        {
            return false;
        }

        return input1 <= condition1;
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
