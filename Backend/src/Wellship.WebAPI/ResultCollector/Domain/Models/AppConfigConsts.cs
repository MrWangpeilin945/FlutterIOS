namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// キーとデフォルト値の定数
/// </summary>
public static class AppConfigConsts
{
    /// <summary>
    /// アクセストークンの有効期限（分）
    /// </summary>
    public static readonly AppConfigSetting<int> AccessTokenLifetime = new() { Key = "Auth.AccessTokenLifetime", DefaultValue = 3 };

    /// <summary>
    /// リフレッシュトークンの有効期限（分）
    /// </summary>
    public static readonly AppConfigSetting<int> RefreshTokenLifeTime = new() { Key = "Auth.RefreshTokenLifeTime", DefaultValue = 720 };

    /// <summary>
    /// トークンのシークレットキー
    /// </summary>
    public static readonly AppConfigSetting<string> SecretKey = new() { Key = "Auth.SecretKey", DefaultValue = "" };
}
