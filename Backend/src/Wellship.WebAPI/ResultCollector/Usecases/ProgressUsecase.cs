using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 進捗ユースケース
/// /// </summary>
public class ProgressUsecase : IProgressUsecase
{
    private readonly IProgressRepository _progressRepository;
    private readonly IPlaceScheduleRepository _placeScheduleRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="progressRepository">進捗リポジトリ</param>
    /// <param name="placeScheduleRepository">会場日程リポジトリ</param>
    public ProgressUsecase(IProgressRepository progressRepository, IPlaceScheduleRepository placeScheduleRepository)
    {
        _progressRepository = progressRepository;
        _placeScheduleRepository = placeScheduleRepository;
    }

    /// <summary>
    /// AP1015_会場日程IDを指定して進捗状況を取得する
    /// </summary>
    public async Task<PlaceScheduleProgress> GetProgressAsync(Guid placeScheduleId)
    {
        var progress = await _progressRepository.GetAggregatedProgressAsync(placeScheduleId);

        // 会場や健診日を取得するために会場日程を取得する
        var placeSchedule = await _placeScheduleRepository.GetPlaceScheduleAsync(progress.PlaceScheduleId);

        return new PlaceScheduleProgress()
        {
            PlaceScheduleId = placeSchedule.Id,
            PlaceName = placeSchedule.Place.Name,
            ExamDate = placeSchedule.ExamDate,
            Progress = progress.AggregatedProgressDetails.Select(x => new Progress()
            {
                ExamMenuId = x.ExamMenuId,
                ExamMenuName = x.ExamMenuName,
                Details = [
                    new ProgressDetail(){Status = (int)AggregatedProgressStatus.予定 , StatusName = AggregatedProgressStatus.予定.ToString(), Count = x.Count11},
                    new ProgressDetail(){Status = (int)AggregatedProgressStatus.来場 , StatusName = AggregatedProgressStatus.来場.ToString(), Count = x.Count21},
                    new ProgressDetail(){Status = (int)AggregatedProgressStatus.済 , StatusName = AggregatedProgressStatus.済.ToString(), Count = x.Count41},
                    new ProgressDetail(){Status = (int)AggregatedProgressStatus.中止 , StatusName = AggregatedProgressStatus.中止.ToString(), Count = x.Count51}
                ]
            }).ToArray()
        };
    }
}
