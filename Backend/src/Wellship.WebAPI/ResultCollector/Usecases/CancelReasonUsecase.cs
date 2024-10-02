using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.Core.Exceptions;
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
    /// 中止理由を取得する
    /// </summary>
    public void GetCancelReasons()
    {
        
    }
}
