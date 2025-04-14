using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;

/// <summary>
/// 入力1と入力値2が等しい数値か
/// </summary>
public sealed class NumericInputEqual : TriggerBase
{
    private readonly List<string> _inputValues;
    private readonly InputErrorLevel _inputErrorLevel;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public NumericInputEqual(List<string> inputValues, InputErrorLevel errorLevel)
    {
        _inputValues = inputValues;
        _inputErrorLevel = errorLevel;
    }

    /// <summary>
    /// トリガーの条件を満たすか計算します。
    /// </summary>
    public override bool IsMatch()
    {
        if (_inputValues.Count != 2)
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

        return input1 == input2;
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
