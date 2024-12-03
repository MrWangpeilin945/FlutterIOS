using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// 会場日程リポジトリ
/// </summary>
public interface IPlaceScheduleRepository
{
    /// <summary>
    /// 健診日を指定して会場日程一覧を取得する
    /// </summary>
    public Task<IEnumerable<PlaceSchedule>> GetPlaceSchedulesAsync(DateOnly date);

    /// <summary>
    /// 会場日程IDを指定して会場日程一覧を取得する
    /// </summary>
    public Task<IEnumerable<PlaceSchedule>> GetPlaceSchedulesAsync(int[] placeScheduleIds);

    /// <summary>
    /// 会場日程IDを指定して会場日程を取得する
    /// </summary>
    public Task<PlaceSchedule> GetPlaceScheduleAsync(int placeScheduleId);

    /// <summary>
    /// 会場ロック状態を取得する
    /// </summary>
    public Task<PlaceScheduleStatus> GetPlaceScheduleLockingStatusAsync(int placeScheduleId);

    /// <summary>
    /// 会場ロック状態を更新する
    /// </summary>
    public Task UpdatePlaceScheduleLockingStatusAsync(int placeScheduleId, PlaceScheduleLockingStatus status);
}
