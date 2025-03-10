namespace Ryobi.Wellship.Core.Enums;

/// <summary>
/// ルール発火条件種別
/// いわゆるトリガーです。
/// </summary>
public enum RuleTriggerType
{
    /// <summary>
    /// しきい値を超えるか
    /// TODO: 仮実装のため後から消すこと
    /// </summary>
    ThresholdExceeded = 1,

    /// <summary>
    /// 複数の入力内容が一致しない
    /// TODO: 仮実装のため後から消すこと
    /// </summary>
    AllInputsNotEqual = 2,

    /// <summary>
    /// 入力1と入力2が等しい数値か
    /// </summary>
    NumericInputEqual = 1101,

    /// <summary>
    /// 入力1と入力2が異なる数値か
    /// </summary>
    NumericInputNotEqual = 1102,

    /// <summary>
    /// 入力1が入力2より大きいか
    /// </summary>
    NumericInputGreaterThan = 1103,

    /// <summary>
    /// 入力1が入力2以上か
    /// </summary>
    NumericInputGreaterThanOrEqual = 1104,

    /// <summary>
    /// 入力1が入力2未満か
    /// </summary>
    NumericInputLessThan = 1105,

    /// <summary>
    /// 入力1が入力2以下か
    /// </summary>
    NumericInputLessThanOrEqual = 1106,

    /// <summary>
    /// 入力値の差の絶対値が判定値より大きいか
    /// </summary>
    AbsoluteGreaterThan = 1201,

    /// <summary>
    /// 入力値の差の絶対値が判定値以上か
    /// </summary>
    AbsoluteGreaterThanOrEqual = 1202,

    /// <summary>
    /// 入力値の差の絶対値が判定値未満か
    /// </summary>
    AbsoluteLessThan = 1203,

    /// <summary>
    /// 入力値の差の絶対値が判定値以下か
    /// </summary>
    AbsoluteLessThanOrEqual = 1204,

    /// <summary>
    /// 入力値が判定値リストに含まれるか
    /// </summary>
    StringIn = 2101,

    /// <summary>
    /// 入力値が判定値リストに含まれないか
    /// </summary>
    StringNotIn = 2102,

    /// <summary>
    /// 複数の入力文字列がすべて等しいか
    /// </summary>
    AllStringsEqual = 2201,

    /// <summary>
    /// 複数の入力文字列のうち異なるものがあるか
    /// </summary>
    AnyStringNotEqual = 2202
}
