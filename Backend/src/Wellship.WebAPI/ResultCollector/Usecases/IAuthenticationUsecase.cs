using Ryobi.Wellship.APIModels.Responses;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 認証ユースケースのインターフェース
/// </summary>
public interface IAuthenticationUsecase
{
    /// <summary>
    /// ログインする
    /// </summary>
    public void Login();
}
