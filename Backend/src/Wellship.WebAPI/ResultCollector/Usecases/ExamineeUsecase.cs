using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 受診者ユースケース
/// </summary>
public class ExamineeUsecase : IExamineeUsecase
{
    private readonly IExamineeRepository _examineeRepository;
    private readonly IProgressRepository _progressRepository;
    private readonly IPlaceScheduleRepository _placeScheduleRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="examineeRepository">受診者リポジトリ</param>
    /// <param name="progressRepository">進捗リポジトリ</param>
    /// <param name="placeScheduleRepository">会場日程リポジトリ</param>
    public ExamineeUsecase(IExamineeRepository examineeRepository, IProgressRepository progressRepository, IPlaceScheduleRepository placeScheduleRepository)
    {
        _examineeRepository = examineeRepository;
        _progressRepository = progressRepository;
        _placeScheduleRepository = placeScheduleRepository;
    }

    /// <summary>
    /// AP1024_受診者一覧を取得する
    /// </summary>
    public async Task<ConsultExamineeList> GetConsultExamineesAsync(Guid placeScheduleId, int examMenuId, AggregatedProgressStatus status)
    {
        // 会場日程の対象検査メニューの進捗状況を取得
        var examMenuProgress = await _progressRepository.GetAggregatedProgressByMenuIdAsync(placeScheduleId, examMenuId);

        // 会場や健診日を取得するために会場日程を取得する
        var placeSchedule = await _placeScheduleRepository.GetPlaceScheduleAsync(placeScheduleId);
        
        // // 会場日程ID、検査メニュー、進捗状況に該当する受診ID配列を取得
        var consultIds = await _examineeRepository.GetConsultIdsAsync(placeScheduleId, examMenuId, status);

        // 受診ID配列で受診者一覧を取得
        var consultExaminees = await _examineeRepository.GetConsultExamineesAsync(consultIds.ToArray());

        return new ConsultExamineeList()
        {
            PlaceScheduleId = placeSchedule.Id,
            PlaceName = placeSchedule.Place.Name,
            ExamDate = placeSchedule.ExamDate,
            Progress = new Progress()
            {
                ExamMenuId = examMenuProgress.ExamMenuId,
                ExamMenuName = examMenuProgress.ExamMenuName,
                Details = [
                    new ProgressDetail(){Status = (int)AggregatedProgressStatus.予定 , StatusName = AggregatedProgressStatus.予定.ToString(), Count = examMenuProgress.Count11},
                    new ProgressDetail(){Status = (int)AggregatedProgressStatus.来場 , StatusName = AggregatedProgressStatus.来場.ToString(), Count = examMenuProgress.Count21},
                    new ProgressDetail(){Status = (int)AggregatedProgressStatus.済 , StatusName = AggregatedProgressStatus.済.ToString(), Count = examMenuProgress.Count41},
                    new ProgressDetail(){Status = (int)AggregatedProgressStatus.中止 , StatusName = AggregatedProgressStatus.中止.ToString(), Count = examMenuProgress.Count51}
                ]
            },
            Examinees = consultExaminees.Select(x => new ConsultExaminee()
            {
                ConsultNumber = x.ConsultNumber,
                TicketNumber = x.TicketNumber ?? "",
                KanaName = x.KanaName,
                Sex = (int)x.Sex,
                CheckedInAt = x.CheckedInAt
            }).ToArray()
        };
    }
}