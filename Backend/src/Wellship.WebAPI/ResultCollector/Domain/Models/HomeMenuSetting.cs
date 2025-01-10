using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// ホームメニューの設定
/// </summary>
public class HomeMenuSetting
{

    /// <summary>
    /// 画面パス
    /// </summary>
    public required string Path { get; init; }

    /// <summary>
    /// 利用可能条件：会場日程が選択済み
    /// </summary>
    public required bool PlaceScheduleSelected { get; init; }

    /// <summary>
    /// 利用可能条件：会場ロックが解除されている
    /// </summary>
    public required bool PlaceScheduleUnlocked { get; init; }

    /// <summary>
    /// 利用できるロール
    /// </summary>
    public required IEnumerable<Role> TargetRoles { get; init; }
}
