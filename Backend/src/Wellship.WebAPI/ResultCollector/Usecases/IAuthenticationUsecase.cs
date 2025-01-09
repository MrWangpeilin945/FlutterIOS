namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 認証ユースケースのインターフェース
/// </summary>
public interface IAuthenticationUsecase
{
    /// <summary>
    /// AP1001_ログインする
    /// </summary>
    /// <param name="identifier">ログインID</param>
    /// <param name="password">パスワード</param>
    public ValueTask<(string accessToken, string refreshToken)> LoginStaffAsync(string identifier, string password);
    /// <summary>
    /// アクセストークンをリフレッシュする
    /// </summary>
    /// <param name="accessToken">古くなったアクセストークン</param>
    /// <param name="refreshToken">リフレッシュトークン</param>
    public ValueTask<(string accessToken, string refreshToken)> RefreshAccessTokenAsync(string accessToken, string refreshToken);
}
