using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// ホームメニューユースケース
/// </summary>
public class HomeMenuUsecase : IHomeMenuUsecase
{
    private readonly IHomeMenuRepository _homeMenuRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="homeMenuRepository">ホームメニューリポジトリ</param>
    public HomeMenuUsecase(IHomeMenuRepository homeMenuRepository)
    {
        _homeMenuRepository = homeMenuRepository;
    }

    /// <summary>
    /// ホームメニュー項目を取得する
    /// </summary>
    public async Task<HomeMenuGroupList> GetHomeMenusAsync()
    {
        // ドメインモデルをWebAPIのレスポンスモデルに変換する
        var repoResults = await _homeMenuRepository.GetHomeMenuGroupsAsync();
        var result = new HomeMenuGroupList()
        {
            HomeMenuGroups = repoResults.Select(x => new HomeMenuGroup()
            {
                GroupName = x.GroupName,
                Menus = x.HomeMenus.Select(m => new HomeMenu()
                {
                    MenuName = m.MenuName,
                    Path = m.Path,
                    AvailableConditions = []
                }).ToArray()
            }).ToArray()
        };
        return result;
    }
}
