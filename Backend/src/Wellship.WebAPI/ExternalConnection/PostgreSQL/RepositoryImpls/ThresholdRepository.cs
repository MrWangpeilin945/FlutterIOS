using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.Utilities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Helper;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Transaction;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls
{
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
        public async Task UpsertThresholdsAsync(List<ThresholdEntity> thresholds, DateTime createdAt, string createdBy)
        {
            using var scope = TransactionScopeHelper.GetTransactionScope();
            {
                using var connection = await _dbConnectionProvider.GetOrOpenAsync();
                {
                    // 一時テーブル作成
                    string sqlCreateTempTable = @"
                        create temp table tmp_thresholds(
                            threshold_code text not null
                            , name text not null
                        ) on commit drop;";
                    // 一時テーブル作成 SQL実行
                    await connection.ExecuteAsync(sqlCreateTempTable);

                    // 一時テーブルにINSERT
                    await BulkInsertHelper.BulkInsert(thresholds,
                        threshold =>
                        $"(" +
                        $"{SqlFormatter.EscapeSqlValue(threshold.ThresholdCode)}, " +
                        $"{SqlFormatter.EscapeSqlValue(threshold.Name)}" +
                        $")",
                        "tmp_thresholds",
                        connection);


                    // UPSERT処理
                    string upsertSql = @"   
                        with max_order_number as (
                            select coalesce(max(order_number), 0) as max_number from resultcollector.thresholds
                        )
                        insert into resultcollector.thresholds (threshold_code, name, order_number, created_at, created_by)
                        select
                            threshold_code,
                            name,
                            max_order.max_number + row_number() over(),
                            @CreatedAt,
                            @CreatedBy
                        from tmp_thresholds
                        cross join max_order_number as max_order
                        on conflict (threshold_code)
                        do update set
                            name = excluded.name,
                            created_at = excluded.created_at,
                            created_by = excluded.created_by;
                    ";
                    // UPSERT処理 SQL実行
                    await connection.ExecuteAsync(upsertSql, new { CreatedAt = createdAt, CreatedBy = createdBy });
                }
                
                scope.Complete();
            }
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
}
