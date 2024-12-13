using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// ホームメニューユースケース
/// </summary>
public class HomeMenuUsecase : IHomeMenuUsecase
{
    private readonly IHomeMenuRepository _homeMenuRepository;
    private readonly IPlaceScheduleRepository _placeScheduleRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="homeMenuRepository">ホームメニューリポジトリ</param>
    /// <param name="placeScheduleRepository">会場日程リポジトリ</param>
    public HomeMenuUsecase(IHomeMenuRepository homeMenuRepository, IPlaceScheduleRepository placeScheduleRepository)
    {
        _homeMenuRepository = homeMenuRepository;
        _placeScheduleRepository = placeScheduleRepository;
    }

    /// <summary>
    /// ホームメニュー項目を取得する
    /// </summary>
    public async Task<HomeMenuGroupList> GetHomeMenusAsync(Guid? placeScheduleId)
    {
        // 会場日程IDが渡された場合、会場ロック状況を取得する
        int? status = placeScheduleId switch
        {
            null => null,
            not null => (await _placeScheduleRepository.GetPlaceScheduleLockingStatusAsync((Guid)placeScheduleId)).Status
        };

        // 機能ごとの利用可能条件の設定
        var homeMenuSettings = new Domain.Models.HomeMenuSettings();

        var repoResults = await _homeMenuRepository.GetHomeMenuGroupsAsync();

        // ドメインモデルをWebAPIのレスポンスモデルに変換する
        var result = new HomeMenuGroupList()
        {
            PlaceScheduleLockingStatus = status,
            HomeMenuGroups = repoResults.Select(x => new HomeMenuGroup()
            {
                GroupName = x.GroupName,
                Menus = x.HomeMenus.Select(m => new HomeMenu()
                {
                    MenuName = m.MenuName,
                    Path = m.Path,
                    AvailableConditions = homeMenuSettings.GetAvailableConditions(m.Path)
                }).ToArray()
            }).ToArray()
        };
        return result;
    }
}
