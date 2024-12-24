namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 認証ユースケースのインターフェース
/// </summary>
public interface IAuthenticationUsecase
{
    /// <summary>
    /// ログインする
    /// </summary>
    public ValueTask<string> LoginStaffAsync(string identifier, string password);
}
