using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.Core.Enums;
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
    public async Task<ExportDataList> GetExportTargetResultsAsync()
    {
        var results = await _integrationRepository.GetExportPlaceSchedulesAsync();

        // 会場名や会場ロック状況を表示するため、会場日程を取得する
        var placeScheduleIds = results.Select(x => x.PlaceScheduleId).Distinct().ToArray();
        var placeSchedules = await _placeScheduleRepository.GetPlaceSchedulesAsync(placeScheduleIds);

        // 健診日の降順で会場日程を返却する
        return new ExportDataList()
        {
            ExportData = results.Select(r =>
            {
                var placeSchedule = placeSchedules.Single(p => p.Id == r.PlaceScheduleId);
                return new ExportData()
                {
                    PlaceScheduleId = r.PlaceScheduleId,
                    PlaceName = placeSchedule.Place.Name,
                    PlaceScheduleLockingStatus = (int)placeSchedule.PlaceScheduleLockingStatus,
                    ExamDate = placeSchedule.ExamDate,
                    StartTime = placeSchedule.FormatStartTimeString,
                    Details = [
                        new(){
                            Status = (int)ConsultResultExportStatus.未出力,
                            StatusName = ConsultResultExportStatus.未出力.ToString(),
                            Count = r.ExportStatusCount11
                        },
                        new(){
                            Status = (int)ConsultResultExportStatus.出力保留,
                            StatusName = ConsultResultExportStatus.出力保留.ToString(),
                            Count = r.ExportStatusCount21
                        },
                        new(){
                            Status = (int)ConsultResultExportStatus.出力済み,
                            StatusName = ConsultResultExportStatus.出力済み.ToString(),
                            Count = r.ExportStatusCount31
                        }
                    ]
                };
            }).OrderByDescending(x => x.ExamDate)
              .ThenByDescending(x => x.StartTime)
              .ToArray()
        };
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
                ExportId = x.ExportId,
                PlaceScheduleId = x.PlaceScheduleId,
                PlaceName = placeSchedule.Place.Name,
                PlaceScheduleLockingStatus = (int)placeSchedule.PlaceScheduleLockingStatus,
                ExamDate = placeSchedule.ExamDate,
                DataCount = x.DataCount,
                ExportedAt = x.ExportedAt,
                ExportedBy = x.ExportedBy
            };
        }).OrderByDescending(x => x.ExportedAt)
          .ToArray();

        return new ExportHistoryList
        {
            ExportHistories = exportHistories
        };
    }

    /// <summary>
    /// 出力した結果を未出力に戻す
    /// </summary>
    public async Task UndoExportStatusAsync(Guid exportId)
    {
        await _integrationRepository.UndoExportStatusAsync(exportId);
    }
}
