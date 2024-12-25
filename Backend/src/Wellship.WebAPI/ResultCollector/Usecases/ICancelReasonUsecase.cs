using Ryobi.Wellship.APIModels.Responses;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 中止理由ユースケースのインターフェース
/// </summary>
public interface ICancelReasonUsecase
{
    /// <summary>
    /// AP1011_中止理由を取得する
    /// </summary>
    public Task<CancelReasonList> GetCancelReasonsAsync();
}
