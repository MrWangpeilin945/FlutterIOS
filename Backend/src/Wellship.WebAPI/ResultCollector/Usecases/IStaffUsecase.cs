using Ryobi.Wellship.APIModels.Responses;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 職員ユースケースのインターフェース
/// </summary>
public interface IStaffUsecase
{
    /// <summary>
    /// 職員の情報を取得する
    /// </summary>
    public Task<Staff> GetStaffAsync(int staffId);
}
