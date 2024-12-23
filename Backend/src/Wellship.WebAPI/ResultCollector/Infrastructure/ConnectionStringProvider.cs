using System.Collections.Concurrent;

using Npgsql;

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

/// <summary>
/// 環境変数から接続情報を取得します。
/// </summary>
public class EnvironmentVariableConnectionStringProvider() : IConnectionStringProvider
{
    // NOTE: 接続文字列キャッシュの有無について要検討。アクセスの都度IOを発生させるのは問題があると思います。
    private readonly ConcurrentDictionary<string, Task<string>> _dictionary = new();

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
    /// 環境変数から接続文字列を取得します。
    /// </summary>
    /// <param name="key">テナントのキー情報</param>
    /// <returns>接続文字列</returns>
    private Task<string> GetConnectionStringAsync(string key)
    {
        var userId = Environment.GetEnvironmentVariable("RDS_USER_ID", EnvironmentVariableTarget.Process) ?? throw new ArgumentNullException("環境変数にDB接続情報が設定されていません。");
        var password = Environment.GetEnvironmentVariable("RDS_USER_PASS", EnvironmentVariableTarget.Process) ?? throw new ArgumentNullException("環境変数にDB接続情報が設定されていません。");
        var endpoint = Environment.GetEnvironmentVariable("RDS_ENDPOINT", EnvironmentVariableTarget.Process) ?? throw new ArgumentNullException("環境変数にDB接続情報が設定されていません。");

        // endpointは以下形式の文字列で設定するのでバラす
        // 例：Host=localhost;Port=15433;Database=dev01;
        var parts = endpoint.Split(';');
        var host = parts.FirstOrDefault(p => p.StartsWith("Host="))?[5..];
        var database = parts.FirstOrDefault(p => p.StartsWith("Database="))?[9..];
        var portString = parts.FirstOrDefault(p => p.StartsWith("Port="))?[5..];

        if (!int.TryParse(portString, out var port))
        {
            throw new ArgumentException("ポート番号の形式が不正です。");
        }

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = host,
            Port = port,
            Username = userId,
            Password = password,
            Database = database
        };
        var connectionString = builder.ToString();

        var cs = key switch
        {
            // TODO: 本来はテナントごとのデータベース接続情報を管理する共通データベースをから検索する予定
            // 開発中なのでテナントごとのデータベースに直接接続している
            _ => connectionString
        };
        return Task.FromResult(cs);
    }
}