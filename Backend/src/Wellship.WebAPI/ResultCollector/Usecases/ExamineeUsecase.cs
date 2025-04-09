using Ryobi.Wellship.APIModels.Responses;
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
    public async Task<ConsultExamineeList> GetConsultExamineesAsync(Guid placeScheduleId, int examMenuId, int status)
    {
        // 会場日程、検査メニュー、進捗状況に該当する受診者ID配列を取得
        var consultExamineeIds = await _examineeRepository.GetConsultIdsAsync(placeScheduleId, examMenuId, status);
        // 受診者ID配列で検索
        var consultExaminees = await _examineeRepository.GetConsultExamineesAsync((Guid[])consultExamineeIds);
        return new ConsultExamineeList()
        {
            Examinees = consultExaminees.Select(x => new ConsultExaminee()
            {
                ConsultNumber = x.ConsultNumber,
                TicketNumber = x.TicketNumber,
                KanaName = x.KanaName,
                Sex = (int)x.Sex,
                CreatedAt = x.CreatedAt
            }).ToArray()
        };
    }
}