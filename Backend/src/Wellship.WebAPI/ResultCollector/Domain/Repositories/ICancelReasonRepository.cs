using Ryobi.Wellship.APIModels.Responses;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// 中止理由リポジトリ
/// </summary>
public interface ICancelReasonRepository
{
    /// <summary>
    /// 中止理由を取得する
    /// </summary>
    public Task<IEnumerable<Domain.Models.CancelReason>> GetCancelReasonsAsync();
}
