
using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// 基幹システム連携リポジトリ
/// </summary>
public class IntegrationRepository : IIntegrationRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    /// <summary>
    /// リポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    public IntegrationRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    /// <summary>
    /// 検査結果出力履歴を取得します。 
    /// </summary>
    public async Task<IEnumerable<ExportHistory>> GetExportHistoryAsync()
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            place_schedule_id as PlaceScheduleId
            , data_count as DataCount
            , exported_at as ExportedAt
            , exported_by as ExportedBy
        from
            resultcollector.export_histories 
        order by
            exported_at desc;";

        var response = await connection.QueryAsync<ExportHistory>(sql);
        return response;
    }
}
