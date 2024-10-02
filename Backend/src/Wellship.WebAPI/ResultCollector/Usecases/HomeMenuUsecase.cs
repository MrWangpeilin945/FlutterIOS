using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.Core.Exceptions;
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
    /// ホームメニュー設定を取得する
    /// </summary>
    public void GetHomeMenuSettings()
    {

    }
}
