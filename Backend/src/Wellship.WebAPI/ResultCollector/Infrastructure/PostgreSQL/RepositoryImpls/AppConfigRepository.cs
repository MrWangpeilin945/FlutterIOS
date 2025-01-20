using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// アプリケーション設定リポジトリ
/// /// </summary>
public class AppConfigRepository : IAppConfigRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    /// <summary>
    /// リポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    public AppConfigRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    /// <inheritdoc/>
    public async Task<AppConfigs> GetAllAsync()
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();

        const string sql = @"
        select
            key
            , value 
        from
            resultcollector.app_config;";

        var results = await connection.QueryAsync<KeyValuePair<string, string>>(sql);

        return new AppConfigs(results);
    }
}
