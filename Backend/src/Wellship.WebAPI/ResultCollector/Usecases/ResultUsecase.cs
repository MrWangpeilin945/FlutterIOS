using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 検査結果ユースケース
/// </summary>
public class ResultUsecase : IResultUsecase
{
    private readonly IResultRepository _resultRepository;
    private readonly IConsultRepository _consultRepository;
    private readonly IPlaceScheduleRepository _placeScheduleRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="resultRepository">検査結果リポジトリ</param>
    /// <param name="consultRepository">受診リポジトリ</param>
    /// <param name="placeScheduleRepository">会場日程リポジトリ</param>
    public ResultUsecase(IResultRepository resultRepository, IConsultRepository consultRepository, IPlaceScheduleRepository placeScheduleRepository)
    {
        _resultRepository = resultRepository;
        _consultRepository = consultRepository;
        _placeScheduleRepository = placeScheduleRepository;
    }

    /// <summary>
    /// 検査結果を検証する
    /// </summary>
    public void VerifyResults()
    {

    }

    /// <summary>
    /// 検査結果を登録する
    /// </summary>
    public async Task RegisterResultsAsync(string consultNumber, ResultsRequest results)
    {
        var consult = await _consultRepository.GetConsultAsync(consultNumber);
        // 会場のロック中かを確認
        // TODO: 管理者のみ更新可能 後方作業へ
        var placeSchedule = await _placeScheduleRepository.GetPlaceScheduleLockingStatusAsync(consult.PlaceScheduleId);
        if (placeSchedule?.Status == PlaceScheduleLockingStatus.検査完了)
        {
            // 会場ロック中
            throw new PlaceScheduleLockedException();
        }
        var resultList = results.ExamResults.SelectMany(exam => exam.ExamItemDetails)
                                            .Select(detail => new ExamResultRegisteEntity{
                                                ExamItemDetailId = detail.ExamItemDetailId,
                                                Value = detail.Value
                                            })
                                            .ToArray();
        await _resultRepository.RegisterResultsAsync(consult.ConsultId, resultList);
    }
}
