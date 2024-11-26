
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
    public async Task<PlaceScheduleTeams> GetTeamsAsync(DateOnly date)
    {
        var placeSchedules = await _placeScheduleRepository.GetPlaceSchedulesAsync(date);

        // 班でグループ化する。1日に同じ会場が複数設定できるので、会場でグループ化して1つだけ返す。
        // 班リストは班.表示順の昇順
        // 会場リストは会場.表示順の昇順
        var results = placeSchedules.GroupBy(x => x.Team.Id)
                               .OrderBy(x => x.First().Team.OrderNumber)
                               .Select(x => new PlaceScheduleTeam()
                               {
                                   TeamId = x.First().Team.Id,
                                   TeamName = x.First().Team.Name,
                                   Places = x.GroupBy(p => p.Place.Id) // PlaceIdでグループ化する
                                             .OrderBy(x => x.First().Place.OrderNumber)
                                             .Select(g => new Place()
                                             {
                                                 PlaceId = g.Key,
                                                 PlaceName = g.First().Place.Name
                                             }).ToArray()
                               }).ToArray();

        return new PlaceScheduleTeams()
        {
            Teams = results
        };
    }

    /// <summary>
    /// 班を指定して会場日程を取得する
    /// </summary>
    public Task GetTeamPlaceSchedulesAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 会場ロック状態を取得する
    /// </summary>
    public Task GetPlaceScheduleLockingStatusAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 会場ロック状態を更新する
    /// </summary>
    public Task UpdatePlaceScheduleLockingStatusAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 会場日程の出力状況を変更する
    /// </summary>
    public Task UpdatePlaceScheduleResultExportStatusAsync()
    {
        throw new NotImplementedException();
    }
}
