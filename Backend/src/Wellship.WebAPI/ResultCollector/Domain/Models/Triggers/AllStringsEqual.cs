using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;

/// <summary>
/// 複数の入力文字列がすべて等しいか
/// </summary>
public sealed class AllStringsEqual : TriggerBase
{
    private readonly List<string> _inputValues;
    private readonly InputErrorLevel _inputErrorLevel;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public AllStringsEqual(List<string> inputValues, InputErrorLevel errorLevel)
    {
        _inputValues = inputValues;
        _inputErrorLevel = errorLevel;
    }

    /// <summary>
    /// トリガーの条件を満たすか計算します。
    /// </summary>
    public override bool IsMatch()
    {
        if (_inputValues.Count < 2)
        {
            return false;
        }

        return _inputValues.All(val => val == _inputValues[0]);
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