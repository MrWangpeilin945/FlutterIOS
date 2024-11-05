using System.Collections.Concurrent;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;

/// <summary>
/// 接続文字列を取得するためのプロバイダのインターフェースです。
/// 動作環境によって接続文字列の取得先が異なるため、処理の差異を吸収します。
/// </summary>
public interface IConnectionStringProvider
{
    /// <summary>
    /// 接続文字列を取得します。
    /// </summary>
    /// <param name="key">テナントのキー情報</param>
    /// <returns>接続文字列</returns>
    public ValueTask<string> GetAsync(string key);
}

/// <summary>
/// 接続文字列を取得するためのプロバイダのインターフェースです。
/// サンプルとしてAppSettingsの特定のパスから接続文字列を取得するプロバイダを用意しています。
/// </summary>
public class AppSettingsConnectionStringProvider(IConfiguration configuration) : IConnectionStringProvider
{
    // NOTE: 接続文字列キャッシュの有無について要検討。アクセスの都度IOを発生させるのは問題があると思います。
    private readonly ConcurrentDictionary<string, Task<string>> _dictionary = new();
    private readonly IConfiguration _configuration = configuration;

    /// <inheritdoc/>
    public async ValueTask<string> GetAsync(string key)
    {
        // NOTE: ここではサンプルのため指定されたキーに対して接続文字列を返すメソッドを用意しています。
        var cs = _dictionary.GetOrAdd(key, GetConnectionStringAsync);
        try
        {
            return await cs;
        }
        catch (Exception)
        {
            // NOTE: サンプルのため、異常時には即辞書からRemoveするようにしています。
            // 存在しない設定を取得しようとした場合などはアクセスの都度IOを発生させてしまう問題があります。
            _dictionary.TryRemove(key, out _);
            throw;
        }
    }

    /// <summary>
    /// サンプルのため仮実装のメソッドです。
    /// ネットワークやIOを介して接続文字列を取得するメソッドに見立てています。
    /// </summary>
    /// <param name="key">テナントのキー情報</param>
    /// <returns>接続文字列</returns>
    private Task<string> GetConnectionStringAsync(string key)
    {
        var path = key switch
        {
            "Tenant1" => "AppSettings:ConnectionStrings:Tenant1",
            "Tenant2" => "AppSettings:ConnectionStrings:Tenant2",
            _ => "AppSettings:ConnectionStrings:DefaultConnection"
        };
        var cs = _configuration.GetValue<string>(path) ?? throw new InvalidOperationException();
        return Task.FromResult(cs);
    }
}