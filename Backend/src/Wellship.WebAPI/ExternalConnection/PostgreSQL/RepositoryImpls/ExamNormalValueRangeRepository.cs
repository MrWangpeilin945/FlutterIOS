using Dapper;

using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Transaction;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls
{
    /// <summary>
    /// 基準値範囲リポジトリ
    /// </summary>
    public class ExamNormalValueRangeRepository : IExamNormalValueRangeRepository
    {
        private readonly IDbConnectionProvider _dbConnectionProvider;

        /// <summary>
        /// リポジトリを生成します。
        /// </summary>
        /// <param name="dbConnectionProvider">dbConnectionProvider</param>
        public ExamNormalValueRangeRepository(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        /// <summary>
        /// 基準値（範囲）を登録する
        /// </summary>
        /// <param name="examNormalValueRangeEntities"></param>
        /// <param name="createdAt"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        public async Task UpsertExamNormalValueRangeAsync(List<ExamNormalValueRangeEntity> examNormalValueRangeEntities, DateTimeOffset createdAt, string createdBy)
        {
            using var scope = TransactionScopeHelper.GetTransactionScope();
            using var connection = await _dbConnectionProvider.GetOrOpenAsync();
            {
                // Delete文を実行
                var deleteKeys = examNormalValueRangeEntities.Select(x => x.ThresholdId.ToString() + '-' + x.ExamItemDetailId.ToString())
                                                            .ToArray();
                const string deleteSql = @"
                                delete 
                                    from resultcollector.exam_normal_value_range
                                where 
                                    concat(threshold_id::text, '-', exam_item_detail_id) = any (@DeleteKeys)";
                await connection.ExecuteAsync(deleteSql, new { DeleteKeys = deleteKeys });

                // Insert文を実行
                var insertItems = examNormalValueRangeEntities.Select(e => new
                {
                    Name = e.Name,
                    ThresholdId = e.ThresholdId,
                    ExamItemDetailId = e.ExamItemDetailId,
                    MaxAge = e.MaxAge,
                    MinAge = e.MinAge,
                    TargetSex = e.TargetSex,
                    MaxValue = e.MaxValue,
                    MinValue = e.MinValue,
                    ErrorLevel = e.ErrorLevel,
                    CreatedAt = createdAt,
                    CreatedBy = createdBy
                }).ToArray();
                const string insertSql = @"
                        insert into resultcollector.exam_normal_value_range
                        (
                            name,
                            threshold_id,
                            exam_item_detail_id,
                            max_age,
                            min_age,
                            target_sex,
                            max_value,
                            min_value,
                            error_level,
                            created_at,
                            created_by
                        )
                        values (
                            @Name,
                            @ThresholdId,
                            @ExamItemDetailId,
                            @MaxAge,
                            @MinAge,
                            @TargetSex,
                            @MaxValue,
                            @MinValue,
                            @ErrorLevel,
                            @CreatedAt,
                            @CreatedBy
                        );";
                await connection.ExecuteAsync(insertSql, insertItems);
                scope.Complete();
            }
        }
    }
}
