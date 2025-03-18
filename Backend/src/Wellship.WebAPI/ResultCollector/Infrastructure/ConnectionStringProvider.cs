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
/// 環境変数から接続情報を取得します。
/// </summary>
public class EnvironmentVariableConnectionStringProvider() : IConnectionStringProvider
{
    // NOTE: 接続文字列をテナントキーごとにキャッシュしています。
    private readonly ConcurrentDictionary<string, Task<string>> _dictionary = new();

    /// <inheritdoc/>
    public async ValueTask<string> GetAsync(string key)
    {
        var cs = _dictionary.GetOrAdd(key, GetConnectionStringAsync);
        try
        {
            return await cs;
        }
        catch (Exception)
        {
            // NOTE: 接続文字列の取得異常時にはキャッシュからTaskをRemoveし、次のアクセス時に再実行させます。
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
        var host = Environment.GetEnvironmentVariable("RDS_ENDPOINT_HOST", EnvironmentVariableTarget.Process) ?? throw new ArgumentNullException("環境変数にDB接続情報が設定されていません。");
        var portString = Environment.GetEnvironmentVariable("RDS_ENDPOINT_PORT", EnvironmentVariableTarget.Process) ?? throw new ArgumentNullException("環境変数にDB接続情報が設定されていません。");

        if (!int.TryParse(portString, out var port))
        {
            throw new ArgumentException("ポート番号の形式が不正です。");
        }

        // NOTE:テナントキーをそのままデータベース名として扱うことを前提としています。
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = host,
            Port = port,
            Username = userId,
            Password = password,
            Database = key,
            ApplicationName = $"Wellship.ResultCollector ({key})",
            // TODO テスト用に100→400にプールサイズを上げています。試験後削除します。
            MaxPoolSize = 400
        };
        var connectionString = builder.ToString();
        return Task.FromResult(connectionString);
    }
}