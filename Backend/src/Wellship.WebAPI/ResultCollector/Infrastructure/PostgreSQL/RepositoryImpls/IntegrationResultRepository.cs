using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// 連携処理結果リポジトリ
/// </summary>
public class IntegrationResultRepository : IIntegrationResultRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// リポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    /// <param name="timeProvider">timeProvider</param>
    public IntegrationResultRepository(IDbConnectionProvider dbConnectionProvider, TimeProvider timeProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
        _timeProvider = timeProvider;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<IntegrationResultLog>> GetIntegrationResultLogsAsync(List<Guid> logIds)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();

        const string sql = @"
        select
            log.id as LogId
            , log.result_code as LogResultCode
            , rc.name as LogResultName
            , log.function_code as LogFunctionCode
            , f.name as LogFunctionName
            , log.log_level as LogLevel
            , log.summary as LogSummary
            , log.created_at as LogCreatedAt
            , d.order_number as DetailOrderNumber
            , d.function_code as DetailFunctionCode
            , df.name as DetailFunctionName
            , d.event_source as DetailEventSource
            , d.result_detail_code as ResultDetailCode
            , d.result_detail_message as ResultDetailMessage
            , d.properties as DetailPropertiesText 
        from
            resultcollector.integration_result_logs log 
            inner join resultcollector.integration_result_log_details d 
                on log.id = d.integration_result_log_id 
            inner join resultcollector.integration_functions f 
                on log.function_code = f.code 
            inner join resultcollector.integration_functions df 
                on d.function_code = df.code 
            inner join resultcollector.integration_result_codes rc 
                on log.result_code = rc.code 
        where
            log.id = any (@LogIds) 
        order by
            log.created_at
            , log.id
            , d.order_number;";

        var results = await connection.QueryAsync<IntegrationResultLogEntity>(sql, new { LogIds = logIds });

        return results.GroupBy(x => x.LogId).Select(x =>
        {
            var log = x.First();
            return new IntegrationResultLog
            {
                Id = x.Key,
                ResultCode = log.LogResultCode,
                LogLevel = (LogLevel)log.LogLevel,
                Summary = log.LogSummary,
                ResultCodeName = log.LogResultName,
                OccurredAt = log.LogCreatedAt,
                FunctionName = log.LogFunctionName,
                FunctionCode = log.LogFunctionCode,
                Details = x.Select(d => new IntegrationResultLogDetail
                {
                    OrderNumber = d.DetailOrderNumber,
                    FunctionCode = d.DetailFunctionCode,
                    FunctionName = d.DetailFunctionName,
                    EventSource = d.DetailEventSource,
                    ResultDetailCode = d.ResultDetailCode,
                    ResultDetailMessage = d.ResultDetailMessage,
                    Properties = d.DetailProperties,
                }).OrderBy(d => d.OrderNumber).ToList()
            };
        }).OrderByDescending(x => x.OccurredAt);
    }

    /// <inheritdoc/>
    public async Task SaveIntegrationResultLogAsync(IEnumerable<IntegrationResultLogWriteModel> resultLogs)
    {
        const string createdBy = "ExternalConnection";
        var operationTime = _timeProvider.GetUtcNow();

        var connection = await _dbConnectionProvider.GetOrOpenAsync();

        // 親テーブルにインサートする内容
        var parentLogs = resultLogs.Select(log => new
        {
            Id = log.Id,
            ResultCode = log.ResultCode,
            LogLevel = (int)log.LogLevel,
            FunctionCode = log.FunctionCode,
            Summary = log.Summary,
            CreatedAt = operationTime,
            CreatedBy = createdBy
        });

        // 明細テーブルにインサートする内容
        // 表示順は1から順に採番する
        var childLogs = resultLogs.SelectMany(log => log.Details.Select((detail, index) => new
        {
            IntegrationResultLogId = log.Id,
            FunctionCode = detail.FunctionCode,
            EventSource = detail.EventSource,
            ResultDetailCode = detail.ResultDetailCode,
            ResultDetailMessage = detail.ResultDetailMessage,
            Properties = detail.PropertiesJsonString,
            OrderNumber = index + 1,
            CreatedAt = operationTime,
            CreatedBy = createdBy
        })).ToList();

        const string parentsSql = @"
        insert into resultcollector.integration_result_logs( 
            id,
            result_code,
            log_level,
            function_code,
            summary,
            created_at,
            created_by
        ) 
        values ( 
            @Id,
            @ResultCode,
            @LogLevel,
            @FunctionCode,
            @Summary,
            @CreatedAt,
            @CreatedBy
        );";

        const string childrenSql = @"
        insert into resultcollector.integration_result_log_details( 
            integration_result_log_id,
            order_number,
            function_code,
            event_source,
            result_detail_code,
            result_detail_message,
            properties,
            created_at,
            created_by
        ) 
        values ( 
            @IntegrationResultLogId,
            @OrderNumber,
            @FunctionCode,
            @EventSource,
            @ResultDetailCode,
            @ResultDetailMessage,
            @Properties::jsonb,
            @CreatedAt,
            @CreatedBy
        );";

        await connection.ExecuteAsync(parentsSql, parentLogs);
        await connection.ExecuteAsync(childrenSql, childLogs);
    }
}
