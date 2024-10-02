using Ryobi.Wellship.APIModels.Responses;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 管理者ユースケースのインターフェース
/// </summary>
public interface IAdministratorUsecase
{
    /// <summary>
    /// 管理者の情報を取得する
    /// </summary>
    public void GetAdministrator();
}
