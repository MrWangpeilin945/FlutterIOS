using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;

/// <summary>
/// 入力1と判定1が等しい文字列であるかつ入力2が判定値リストに含まれるか
/// </summary>
public sealed class StringEqualAndContainedInList : TriggerBase
{
    private readonly List<string> _inputValues;
    private readonly InputErrorLevel _inputErrorLevel;
    private readonly List<string> _conditionValues;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public StringEqualAndContainedInList(List<string> inputValues, List<string> conditionValues, InputErrorLevel errorLevel)
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
        if (_inputValues.Count < 2 || _conditionValues.Count < 1)
        {
            return false;
        }

        var input1 = _inputValues[0];
        var input2 = _inputValues[1];
        var condition1 = _conditionValues[0];
        var conditionList = _conditionValues.Skip(1).ToList();

        return input1 == condition1 && conditionList.Contains(input2);
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
