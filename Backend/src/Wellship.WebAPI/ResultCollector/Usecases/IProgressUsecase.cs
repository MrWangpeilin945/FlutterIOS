using Ryobi.Wellship.APIModels.Responses;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 進捗ユースケースのインターフェース
/// </summary>
public interface IProgressUsecase
{
    /// <summary>
    /// 会場日程IDを指定して進捗状況を取得する
    /// </summary>
    public Task<PlaceScheduleProgress> GetProgressAsync(int placeScheduleId);
}