using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;

/// <summary>
/// 入力値が判定值リストに含まれるか
/// </summary>
public sealed class StringIn : TriggerBase
{
    private readonly List<string> _inputValues;
    private readonly List<string> _conditionValues;
    private readonly InputErrorLevel _inputErrorLevel;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public StringIn(List<string> inputValues, List<string> conditionValues, InputErrorLevel errorLevel)
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
        if (_inputValues.Count < 1)
        {
            return false;
        }
        if (_conditionValues.Count < 1)
        {
            return false;
        }

        return _conditionValues.Contains(_inputValues[0]);
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
