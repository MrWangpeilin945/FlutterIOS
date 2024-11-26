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
    public Task<PlaceScheduleTeams> GetTeamsAsync(DateOnly date);

    /// <summary>
    /// 班を指定して会場日程を取得する
    /// </summary>
    public Task<PlaceSchedulePlaces> GetTeamPlaceSchedulesAsync(DateOnly examDate, int teamId);

    /// <summary>
    /// 会場ロック状態を取得する
    /// </summary>
    public Task GetPlaceScheduleLockingStatusAsync();

    /// <summary>
    /// 会場ロック状態を更新する
    /// </summary>
    public Task UpdatePlaceScheduleLockingStatusAsync();

    /// <summary>
    /// 会場日程の出力状況を変更する
    /// </summary>
    public Task UpdatePlaceScheduleResultExportStatusAsync();
}
