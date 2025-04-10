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

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="examineeRepository">受診者リポジトリ</param>
    public ExamineeUsecase(IExamineeRepository examineeRepository)
    {
        _examineeRepository = examineeRepository;
    }

    /// <summary>
    /// AP1024_受診者一覧を取得する
    /// </summary>
    public async Task<ConsultExamineeList> GetConsultExamineesAsync(Guid placeScheduleId, int examMenuId, AggregatedProgressStatus status)
    {
        // 会場日程、検査メニュー、進捗状況に該当する受診ID配列を取得
        var consultIds = await _examineeRepository.GetConsultIdsAsync(placeScheduleId, examMenuId, status);
        // 受診ID配列で受診者一覧を取得
        var consultExaminees = await _examineeRepository.GetConsultExamineesAsync(consultIds.ToArray());
        return new ConsultExamineeList()
        {
            Examinees = consultExaminees.Select(x => new ConsultExaminee()
            {
                ConsultNumber = x.ConsultNumber,
                TicketNumber = x.TicketNumber ?? "",
                KanaName = x.KanaName,
                Sex = (int)x.Sex,
                CreatedAt = x.CreatedAt
            }).ToArray()
        };
    }
}