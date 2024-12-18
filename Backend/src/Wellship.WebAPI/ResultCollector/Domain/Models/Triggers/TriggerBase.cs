using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;

/// <summary>
/// トリガーの基底クラス
/// 共通処理をまとめます。 
/// </summary>
public abstract class TriggerBase : ITrigger
{
    /// <summary>
    /// トリガーの条件を満たすか計算します。
    /// </summary>
    public abstract bool IsMatch();

    /// <summary>
    /// 入力値を検証して入力エラーレベルを取得します。
    /// </summary>
    public abstract InputErrorLevel GetErrorLevel();

    /// <summary>
    /// 文字列を数値型に変換します。
    /// </summary>
    /// <param name="inputString"></param>
    public decimal StringToDecimal(string inputString)
    {
        if (decimal.TryParse(inputString, out var decimalValue))
        {
            return decimalValue;
        }
        throw new ArgumentException(inputString, "文字列を数値型に変換できません。");
    }
}
