using Ryobi.Wellship.APIModels.Responses;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 健診メニューユースケースのインターフェース
/// </summary>
public interface ICheckupMenuUsecase
{
    /// <summary>
    /// 健診メニュー一覧を取得する
    /// </summary>
    public void GetCheckupMenus();
}