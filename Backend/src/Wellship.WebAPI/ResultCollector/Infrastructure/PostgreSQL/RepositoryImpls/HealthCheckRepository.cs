using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.RepositoryImpls;

/// <summary>
/// ヘルスチェック用のリポジトリ
/// </summary>
public class HealthCheckRepository : IHealthCheckRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    /// <summary>
    /// ヘルスチェック用のリポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    public HealthCheckRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    /// <summary>
    /// SELECT 1を投げてDB接続を確認します。
    /// </summary>
    /// <returns>接続が正常か</returns>
    public async Task<bool> CheckDatabaseConnectionAsync()
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = "SELECT 1;";
        var result = await connection.QueryAsync<int>(sql);
        return result.SingleOrDefault() == 1;
    }
}
