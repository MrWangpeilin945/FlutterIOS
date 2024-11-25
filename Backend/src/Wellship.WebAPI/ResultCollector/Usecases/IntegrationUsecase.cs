using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 基幹システム連携ユースケース
/// </summary>
public class IntegrationUsecase : IIntegrationUsecase
{
    private readonly IIntegrationRepository _integrationRepository;
    private readonly IPlaceScheduleRepository _placeScheduleRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public IntegrationUsecase(IIntegrationRepository integrationRepository, IPlaceScheduleRepository placeScheduleRepository)
    {
        _integrationRepository = integrationRepository;
        _placeScheduleRepository = placeScheduleRepository;
    }

    /// <summary>
    /// 連携対象の検査結果を取得する
    /// </summary>
    public void GetIntegrationResults()
    {

    }

    /// <summary>
    /// 連携用に検査結果を出力する
    /// </summary>
    public void ExportResults()
    {

    }

    /// <summary>
    /// 検査結果の出力履歴を取得する
    /// </summary>
    public async Task<ExportHistoryList> GetExportHistoryAsync()
    {
        var histories = await _integrationRepository.GetExportHistoryAsync();

        // 会場名や会場ロック状況を表示するため、会場日程を取得する
        var placeScheduleIds = histories.Select(x => x.PlaceScheduleId).Distinct().ToArray();
        var placeSchedules = await _placeScheduleRepository.GetPlaceSchedulesAsync(placeScheduleIds);

        var exportHistories = histories.Select(x =>
        {
            var placeSchedule = placeSchedules.Single(p => p.Id == x.PlaceScheduleId);

            return new ExportHistory
            {
                PlaceScheduleId = x.PlaceScheduleId,
                PlaceName = placeSchedule.Place.Name,
                PlaceScheduleLockingStatus = (int)placeSchedule.PlaceScheduleLockingStatus,
                ExamDate = placeSchedule.ExamDate,
                DataCount = x.DataCount,
                ExportedAt = x.ExportedAt,
                ExportedBy = x.ExportedBy
            };
        }).OrderByDescending(x => x.ExportedAt).ToArray();

        return new ExportHistoryList
        {
            ExportHistories = exportHistories
        };
    }
}
