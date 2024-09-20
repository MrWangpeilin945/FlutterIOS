using System.Data;

using Dapper;

using Npgsql;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL;

/// <summary>
/// PostgreSQLに接続するためのコネクタ
/// </summary>
public class PostgresConnector
{
    private readonly string _connectionString;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PostgresConnector(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("接続文字列が設定されていません。");
        }
        _connectionString = connectionString;
    }

    /// <summary>
    /// コネクションを作成します。
    /// </summary>
    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }

    /// <summary>
    /// クエリを実行して結果を取得します。
    /// </summary>
    /// <param name="sql">SQL</param>
    /// <param name="parameters">パラメータ</param>
    /// <typeparam name="T">取得してマッピングする型</typeparam>
    public IEnumerable<T> Query<T>(string sql, object? parameters = null)
    {
        using var connection = CreateConnection();
        return connection.Query<T>(sql, parameters);
    }

    /// <summary>
    /// クエリを実行します。
    /// </summary>
    /// <param name="sql">SQL</param>
    /// <param name="parameters">パラメータ</param>
    /// <param name="transaction">トランザクション</param>
    public void Execute(string sql, object? parameters = null, IDbTransaction? transaction = null)
    {
        using var connection = CreateConnection();
        connection.Execute(sql, parameters, transaction);
    }
}
