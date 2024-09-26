using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.RepositoryImpls;

/// <summary>
/// ヘルスチェック用のリポジトリ
/// </summary>
public class HealthCheckRepository : IHealthCheckRepository
{
    private readonly PostgresConnector _connector;

    /// <summary>
    /// ヘルスチェック用のリポジトリを生成します。
    /// </summary>
    /// <param name="connector">PostgreSQL用コネクター</param>
    public HealthCheckRepository(PostgresConnector connector)
    {
        _connector = connector;
    }

    /// <summary>
    /// SELECT 1を投げてDB接続を確認します。
    /// </summary>
    /// <returns>接続が正常か</returns>
    public bool CheckDatabaseConnection()
    {
        var sql = "SELECT 1;";
        var result = _connector.Query<int>(sql).SingleOrDefault();
        return result == 1;
    }
}
