
using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 会場日程ユースケース
/// </summary>
public class PlaceScheduleUsecase : IPlaceScheduleUsecase
{
    private readonly IPlaceScheduleRepository _placeScheduleRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PlaceScheduleUsecase(IPlaceScheduleRepository placeScheduleRepository)
    {
        _placeScheduleRepository = placeScheduleRepository;
    }

    /// <summary>
    /// 班リストを取得する
    /// </summary>
    /// <param name="date">健診日</param>
    public PlaceScheduleTeams GetTeams(DateOnly date)
    {
        var placeSchedules = _placeScheduleRepository.GetPlaceSchedules(date);

        // 班でグループ化する
        var results = placeSchedules.GroupBy(x => x.Team.Id)
                                    .Select(x => new PlaceScheduleTeam(x.Key,
                                                                       x.First().Team.Name,
                                                                       x.Select(p => new Place(p.Place.Id,
                                                                                               p.Place.Name)).ToList())).ToArray();
        return new PlaceScheduleTeams(results);
    }

    /// <summary>
    /// 班を指定して会場日程を取得する
    /// </summary>
    public void GetTeamPlaceSchedules()
    {

    }

    /// <summary>
    /// 会場状況を取得する
    /// </summary>
    public void GetPlaceStatus()
    {

    }

    /// <summary>
    /// 会場状況を更新する
    /// </summary>
    public void UpdatePlaceStatus()
    {

    }
}
