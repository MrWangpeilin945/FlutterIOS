namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// ホームメニューグループ
/// </summary>
public class HomeMenuGroup
{
    /// <summary>
    /// ホームメニューグループID
    /// </summary>
    public required int GroupId { get; init; }

    /// <summary>
    /// ホームメニューグループ名
    /// </summary>
    public required string GroupName { get; init; }

    /// <summary>
    /// ホームメニューリスト
    /// </summary>
    public required IEnumerable<HomeMenu> HomeMenus { get; init; }
}