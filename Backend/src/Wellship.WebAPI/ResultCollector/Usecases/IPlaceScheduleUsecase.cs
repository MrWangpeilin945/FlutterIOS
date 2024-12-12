using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.Core.Enums;

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
    public Task<PlaceScheduleTeams> GetTeamsAsync(DateOnly date);

    /// <summary>
    /// 班を指定して会場日程を取得する
    /// </summary>
    public Task<PlaceSchedulePlaces> GetTeamPlaceSchedulesAsync(DateOnly examDate, Guid teamId);

    /// <summary>
    /// 会場ロック状態を取得する
    /// </summary>
    public Task<PlaceScheduleLocking> GetPlaceScheduleLockingStatusAsync(Guid placeScheduleId);

    /// <summary>
    /// 会場ロック状態を更新する
    /// </summary>
    public Task UpdatePlaceScheduleLockingStatusAsync(Guid placeScheduleId, PlaceScheduleLockingStatus status);
}
