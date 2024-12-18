using Ryobi.Wellship.APIModels.Responses;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// ホームメニューユースケースのインターフェース
/// </summary>
public interface IHomeMenuUsecase
{
    /// <summary>
    /// ホームメニュー項目を取得する
    /// </summary>
    public Task<HomeMenuGroupList> GetHomeMenusAsync(Guid? placeScheduleId);
}