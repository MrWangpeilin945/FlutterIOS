using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 認証ユースケース
/// </summary>
public class AuthenticationUsecase : IAuthenticationUsecase
{
    private readonly IAuthenticationRepository _authenticationRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="authenticationRepository">認証リポジトリ</param>
    public AuthenticationUsecase(IAuthenticationRepository authenticationRepository)
    {
        _authenticationRepository = authenticationRepository;
    }

    /// <summary>
    /// ログインする
    /// </summary>
    public void Login()
    {

    }
}
