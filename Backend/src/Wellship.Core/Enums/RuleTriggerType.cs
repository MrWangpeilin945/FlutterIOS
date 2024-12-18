namespace Ryobi.Wellship.Core.Enums;

/// <summary>
/// ルール発火条件種別
/// いわゆるトリガーです。
/// </summary>
public enum RuleTriggerType
{
    /// <summary>
    /// しきい値を超えるか
    /// </summary>
    ThresholdExceeded = 1,

    /// <summary>
    /// 複数の入力内容が一致しない
    /// </summary>
    AllInputsNotEqual = 2
}
