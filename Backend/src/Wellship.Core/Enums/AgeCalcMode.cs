namespace Ryobi.Wellship.Core.Enums;

/// <summary>
/// 年齢計算モード（加算方法）
/// </summary>
public enum AgeCalcMode
{
    /// <summary>
    /// 誕生日前日に年齢を加算する（民法第143条)
    /// </summary>
    前日年齢加算 = 0,

    /// <summary>
    /// 誕生日当日に年齢を加算する
    /// </summary>
    当日年齢加算 = 1
}
