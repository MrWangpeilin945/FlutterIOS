using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

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
    public async Task SaveIntegrationResultLogAsync(IEnumerable<IntegrationResultLogWriteModel> resultLogs)
    {
        const string createdBy = "ExternalConnection";
        var operationTime = _timeProvider.GetUtcNow();

        var connection = await _dbConnectionProvider.GetOrOpenAsync();

        // 処理結果コードのログレベルを取得
        var resultCodes = resultLogs.Select(log => log.ResultCode).Distinct();
        const string logLevelSql = @"
        select
            code as ResultCode
            , log_level as LogLevel 
        from
            resultcollector.integration_result_codes 
        where
            code = any (@ResultCodes);";
        var logLevels = await connection.QueryAsync<(string ResultCode, int LogLevel)>(logLevelSql, new { ResultCodes = resultCodes });
        var logLevelDict = logLevels.ToDictionary(x => x.ResultCode, x => x.LogLevel);

        // 親テーブルにインサートする内容
        var parentLogs = resultLogs.Select(log => new
        {
            Id = log.Id,
            ResultCode = log.ResultCode,
            LogLevel = logLevelDict[log.ResultCode],
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
