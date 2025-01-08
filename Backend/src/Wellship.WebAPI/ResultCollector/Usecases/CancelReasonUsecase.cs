using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 中止理由ユースケース
/// </summary>
public class CancelReasonUsecase : ICancelReasonUsecase
{
    private readonly ICancelReasonRepository _cancelReasonRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="cancelReasonRepository">中止理由リポジトリ</param>
    public CancelReasonUsecase(ICancelReasonRepository cancelReasonRepository)
    {
        _cancelReasonRepository = cancelReasonRepository;
    }

    /// <summary>
    /// AP1011_中止理由を取得する
    /// </summary>
    public async Task<CancelReasonList> GetCancelReasonsAsync()
    {
        var results = await _cancelReasonRepository.GetCancelReasonsAsync();

        return new CancelReasonList()
        {
            CancelReasons = results.Select(x => new CancelReason()
            {
                CancelReasonId = x.CancelReasonId,
                CancelReasonName = x.Name,
                ExamItemId = x.ExamItemId
            }).ToArray()
        };
    }
}
