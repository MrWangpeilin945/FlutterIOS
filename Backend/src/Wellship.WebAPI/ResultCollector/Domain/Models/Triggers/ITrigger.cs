using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;

/// <summary>
/// トリガーのインターフェース
/// 各種チェックで使用するための条件
/// </summary>
public interface ITrigger
{
    /// <summary>
    /// トリガーの条件を満たすか計算します。
    /// </summary>
    public bool IsMatch();

    /// <summary>
    /// 入力値を検証して入力エラーレベルを取得します。
    /// </summary>
    public InputErrorLevel GetErrorLevel();
}
