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
}
