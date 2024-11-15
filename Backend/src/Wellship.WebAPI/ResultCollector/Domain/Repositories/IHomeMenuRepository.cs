namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// ホームメニューリポジトリ
/// </summary>
public interface IHomeMenuRepository
{
    /// <summary>
    /// ホームメニューグループ一覧を取得します。
    /// </summary>
    public Task<IEnumerable<Models.HomeMenuGroup>> GetHomeMenuGroupsAsync();
}
