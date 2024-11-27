

using Dapper;

using Ryobi.Wellship.Core.Enums;
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
            h.id as ExportId
            , place_schedule_id as PlaceScheduleId
            , COUNT(d.consult_id) as DataCount
            , exported_at as ExportedAt
            , exported_by as ExportedBy 
        from
            resultcollector.export_histories as h 
            left join resultcollector.export_history_details as d 
                on h.id = d.id 
        group by
            h.id 
        order by
            h.exported_at desc;";

        var response = await connection.QueryAsync<ExportHistory>(sql);
        return response;
    }

    /// <summary>
    /// 検査結果出力のために会場日程ごとの受診を取得する
    /// </summary>
    public async Task<IEnumerable<ExportPlaceSchedule>> GetExportPlaceSchedulesAsync()
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            place_schedule_id as PlaceScheduleId
            , SUM(case when export_status = 11 then 1 else 0 end) as ExportStatusCount11
            , SUM(case when export_status = 21 then 1 else 0 end) as ExportStatusCount21
            , SUM(case when export_status = 31 then 1 else 0 end) as ExportStatusCount31 
        from
            resultcollector.consult 
        group by
            place_schedule_id;";

        var response = await connection.QueryAsync<ExportPlaceSchedule>(sql);
        return response;
    }

    /// <summary>
    /// 出力履歴IDを指定して受診の出力状況を未出力に戻す
    /// </summary>
    public async Task UndoExportStatusAsync(Guid exportId)
    {
        var status = ConsultResultExportStatus.未出力;

        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        update resultcollector.consult 
        set
            export_status = @Status
        where
            consult_id in ( 
                select
                    consult_id 
                from
                    resultcollector.export_history_details 
                where
                    id = @ExportId
            );";
        await connection.QueryAsync(sql, new { Status = status, ExportId = exportId });
    }
}
