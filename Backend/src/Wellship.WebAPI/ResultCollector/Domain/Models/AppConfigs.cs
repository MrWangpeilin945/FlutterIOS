namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// アプリケーション設定のリスト
/// 取得できなかった場合と数値等に変換できなかった場合のふるまいも定義します。
/// </summary>
public class AppConfigs
{
    private readonly Dictionary<string, string> _appConfigs;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public AppConfigs(IEnumerable<KeyValuePair<string, string>> appConfigs)
    {
        _appConfigs = appConfigs.ToDictionary();
    }

    /// <summary>
    /// アクセストークンの有効期限（分）
    /// </summary>
    /// <value></value>
    public int AccessTokenLifetime
    {
        get
        {
            if (!_appConfigs.TryGetValue(AppConfigConsts.AccessTokenLifetime.Key, out var resultString))
            {
                return AppConfigConsts.AccessTokenLifetime.DefaultValue;
            }

            if (int.TryParse(resultString, out var result))
            {
                return result;
            }

            return AppConfigConsts.AccessTokenLifetime.DefaultValue;
        }
    }

    /// <summary>
    /// リフレッシュトークンの有効期限（分）
    /// </summary>
    public int RefreshTokenLifeTime
    {
        get
        {
            if (!_appConfigs.TryGetValue(AppConfigConsts.RefreshTokenLifeTime.Key, out var resultString))
            {
                return AppConfigConsts.RefreshTokenLifeTime.DefaultValue;
            }

            if (int.TryParse(resultString, out var result))
            {
                return result;
            }

            return AppConfigConsts.RefreshTokenLifeTime.DefaultValue;
        }
    }

    /// <summary>
    /// トークンのシークレットキー
    /// </summary>
    public string SecretKey
    {
        get
        {
            if (!_appConfigs.TryGetValue(AppConfigConsts.SecretKey.Key, out var resultString))
            {
                throw new ArgumentNullException(nameof(resultString), "SecretKeyは必須設定です。");
            }

            return resultString;
        }
    }
}
