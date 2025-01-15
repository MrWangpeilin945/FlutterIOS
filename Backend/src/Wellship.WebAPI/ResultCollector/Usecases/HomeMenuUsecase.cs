using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// ホームメニューユースケース
/// </summary>
public class HomeMenuUsecase : IHomeMenuUsecase
{
    private readonly IHomeMenuRepository _homeMenuRepository;
    private readonly IPlaceScheduleRepository _placeScheduleRepository;
    private readonly IStaffIdentityProvider _staffIdentityProvider;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="homeMenuRepository">ホームメニューリポジトリ</param>
    /// <param name="placeScheduleRepository">会場日程リポジトリ</param>
    /// <param name="staffIdentityProvider">職員情報プロバイダ</param>
    public HomeMenuUsecase(IHomeMenuRepository homeMenuRepository, IPlaceScheduleRepository placeScheduleRepository, IStaffIdentityProvider staffIdentityProvider)
    {
        _homeMenuRepository = homeMenuRepository;
        _placeScheduleRepository = placeScheduleRepository;
        _staffIdentityProvider = staffIdentityProvider;
    }

    /// <summary>
    /// AP1005_ホームメニュー項目を取得する
    /// </summary>
    public async Task<HomeMenuGroupList> GetHomeMenusAsync(Guid? placeScheduleId)
    {
        // 会場日程IDが渡された場合、会場ロック状況を取得する
        int? status = placeScheduleId switch
        {
            null => null,
            not null => (int)(await _placeScheduleRepository.GetPlaceScheduleLockingStatusAsync((Guid)placeScheduleId)).Status
        };

        var staffRole = _staffIdentityProvider.Role;
        if (staffRole is null)
        {
            throw new WellshipAuthenticationException();
        }

        // 機能ごとの利用可能条件の設定
        var homeMenuSettings = new Domain.Models.HomeMenuSettings();

        var repoResults = await _homeMenuRepository.GetHomeMenuGroupsAsync();

        // ドメインモデルをWebAPIのレスポンスモデルに変換する
        var result = new HomeMenuGroupList()
        {
            PlaceScheduleLockingStatus = status,
            StaffRole = (int)staffRole,
            HomeMenuGroups = repoResults.Select(x => new HomeMenuGroup()
            {
                GroupName = x.GroupName,
                Menus = x.HomeMenus.Where(m => homeMenuSettings.CanRoleUseFeature(m.Path, (Role)staffRole))
                                   .Select(m => new HomeMenu()
                                   {
                                       MenuName = m.MenuName,
                                       Path = m.Path,
                                       AvailableConditions = homeMenuSettings.GetAvailableConditions(m.Path)
                                   }).ToArray()
            }).Where(x => x.Menus.Length > 0).ToArray()
        };
        return result;
    }
}
