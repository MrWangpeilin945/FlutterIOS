using System.Collections.Concurrent;
using System.Data.Common;

using Npgsql;

using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL;


/// <summary>
/// データベースのデータソースを管理するクラス
/// </summary>
/// <param name="connectionStringProvider">接続文字列を取得するためのプロバイダ</param>
/// <param name="loggerFactory">ロガーのファクトリ</param>
public class NpgsqlDbDataSourceRegistry(IConnectionStringProvider connectionStringProvider, ILoggerFactory loggerFactory) : IDbDataSourceRegistry
{
    private readonly ConcurrentDictionary<string, Task<DbDataSource>> _sources = new();
    private readonly IConnectionStringProvider _connectionStringProvider = connectionStringProvider;
    private readonly ILoggerFactory _loggerFactory = loggerFactory;

    /// <inheritdoc/>
    public async ValueTask<DbDataSource> GetOrCreateAsync(string key)
    {
        var dataSourceTask = _sources.GetOrAdd(key, CreateDbDataSourceAsync);
        try
        {
            // NOTE: タスクの処理に失敗したときその場でリムーブしたいためawaitしています。
            return await dataSourceTask;
        }
        catch (Exception)
        {
            // 実行したタスクが失敗している場合はその場でリムーブします。
            _sources.TryRemove(key, out _);
            throw;
        }
    }

    /// <inheritdoc/>
    private async Task<DbDataSource> CreateDbDataSourceAsync(string key)
    {
        var connectionString = await _connectionStringProvider.GetAsync(key);
        return new NpgsqlDataSourceBuilder(connectionString)
               .UseLoggerFactory(_loggerFactory)
               .Build();
    }
}