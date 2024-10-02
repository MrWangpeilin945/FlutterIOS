using Ryobi.Wellship.APIModels.Responses;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 会場日程ユースケースのインターフェース
/// </summary>
public interface IPlaceScheduleUsecase
{
    /// <summary>
    /// 班リストを取得する
    /// </summary>
    /// <param name="date">健診日</param>
    public PlaceScheduleTeams GetTeams(DateOnly date);

    /// <summary>
    /// 班を指定して会場日程を取得する
    /// </summary>
    public void GetTeamPlaceSchedules();

    /// <summary>
    /// 会場状況を取得する
    /// </summary>
    public void GetPlaceStatus();

    /// <summary>
    /// 会場状況を更新する
    /// </summary>
    public void UpdatePlaceStatus();
}
