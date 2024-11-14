namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// ホームメニュー
/// </summary>
public class HomeMenu
{
    /// <summary>
    /// ホームメニューID
    /// </summary>
    public required int MenuId { get; init; }

    /// <summary>
    /// ホームメニュー名
    /// </summary>
    public required string MenuName { get; init; }

    /// <summary>
    /// 画面パス
    /// </summary>
    public required string Path { get; init; }
}
