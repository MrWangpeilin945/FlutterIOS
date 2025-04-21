using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Transaction;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;

/// <summary>
/// 基準パターンを登録するRepository層
/// </summary>
public class ThresholdRepository : IThresholdRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    /// <summary>
    /// リポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    public ThresholdRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    /// <summary>
    /// 基準パターンを登録するRepository層
    /// </summary>
    /// <param name="thresholds">基準パターン</param>
    /// <param name="createdAt">作成日時</param>
    /// <param name="createdBy">作成者</param>
    public async Task UpsertThresholdsAsync(List<ThresholdEntity> thresholds, DateTimeOffset createdAt, string createdBy)
    {
        using var scope = TransactionScopeHelper.GetTransactionScope();
        using var connection = await _dbConnectionProvider.GetOrOpenAsync();
        // 表示順を取得する
        const string selectOrderNumberSql = @"
        select 
            coalesce(max(order_number), 0) + 1
        from 
            resultcollector.thresholds";
        var result = await connection.QueryAsync<int>(selectOrderNumberSql);
        int orderNumber = result.FirstOrDefault();

        var upsertThresholds = thresholds.Select((x, index) => new
        {
            ThresholdId = Guid.NewGuid(),
            ThresholdCode = x.ThresholdCode,
            Name = x.Name,
            OrderNumber = orderNumber + index,
            CreatedAt = createdAt,
            CreatedBy = createdBy,
        }).ToArray();

        // thresholds（基準値パターン）
        const string mergeThresholdSql = @"
        merge
        into resultcollector.thresholds as threshold
            using (values (@ThresholdId, @ThresholdCode, @Name, @OrderNumber,
                            @CreatedAt, @CreatedBy)) as new_data(
                threshold_id
                , threshold_code
                , name
                , order_number
                , created_at
                , created_by
            )
            on threshold.threshold_code = new_data.threshold_code
        when matched then 
            update set
                name = new_data.name
                , created_at = new_data.created_at
                , created_by = new_data.created_by 
        when not matched then
            insert (
                threshold_id
                , threshold_code
                , name
                , order_number
                , created_at
                , created_by
            )
            values (
                new_data.threshold_id
                , new_data.threshold_code
                , new_data.name
                , new_data.order_number
                , new_data.created_at
                , new_data.created_by
            );";
        await connection.ExecuteAsync(mergeThresholdSql, upsertThresholds);
        scope.Complete();
    }

    /// <summary>
    /// 基準値コードの取得
    /// </summary>
    /// <param name="thresholdCodes"></param>
    /// <returns></returns>
    public async Task<List<ThresholdEntity>> GetThresholdsByCodesAsync(List<string> thresholdCodes)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        var sql = @"
                select
                    threshold_id as ThresholdId, threshold_code as ThresholdCode
                from
                    resultcollector.thresholds
                where
                    threshold_code = any(@ThresholdCodes);";

        var result = await connection.QueryAsync<ThresholdEntity>(sql, new { ThresholdCodes = thresholdCodes.ToArray() });

        return result.ToList();
    }
}
