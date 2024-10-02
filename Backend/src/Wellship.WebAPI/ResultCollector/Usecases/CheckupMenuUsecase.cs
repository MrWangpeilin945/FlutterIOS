using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 健診メニューユースケース
/// </summary>
public class CheckupMenuUsecase : ICheckupMenuUsecase
{
    private readonly ICheckupMenuRepository _checkupMenuRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="checkupMenuRepository">健診メニューリポジトリ</param>
    public CheckupMenuUsecase(ICheckupMenuRepository checkupMenuRepository)
    {
        _checkupMenuRepository = checkupMenuRepository;
    }

    /// <summary>
    /// 健診メニュー一覧を取得する
    /// </summary>
    public void GetCheckupMenus()
    {

    }
}
