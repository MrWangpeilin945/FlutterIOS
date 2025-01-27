using Dapper;

using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using System.Data.Common;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using System.Numerics;


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
        public async Task UpsertExamNormalValueRangeAsync(List<ExamNormalValueRangeEntity> examNormalValueRangeEntities, DateTime createdAt, string createdBy)
        {
            var connection = await _dbConnectionProvider.GetOrOpenAsync();
            var transaction = await connection.BeginTransactionAsync();
            try
            {
                var upsertItems = examNormalValueRangeEntities.Select(e => new
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

                // Upsert文を実行
                string mergeSql = @"
                        merge
                        into resultcollector.exam_normal_value_range as range
                            using (values (@Name, @ThresholdId, @ExamItemDetailId, @MaxAge, @MinAge, @TargetSex, @MaxValue, @MinValue, @ErrorLevel, @CreatedAt, @CreatedBy)) as new_data (
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
                                on range.threshold_id = new_data.threshold_id
                                and range.exam_item_detail_id = new_data.exam_item_detail_id
                                and range.max_age = new_data.max_age
                                and range.target_sex = new_data.target_sex
                                and range.max_value = new_data.max_value
                        when matched then update
                        set
                            name = new_data.name,
                            min_age = new_data.min_age,
                            min_value = new_data.min_value,
                            error_level = new_data.error_level,
                            created_at = new_data.created_at,
                            created_by = new_data.created_by 
                        when not matched then
                        insert (
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
                            new_data.name,
                            new_data.threshold_id,
                            new_data.exam_item_detail_id,
                            new_data.max_age,
                            new_data.min_age,
                            new_data.target_sex,
                            new_data.max_value,
                            new_data.min_value,
                            new_data.error_level,
                            new_data.created_at,
                            new_data.created_by
                        );";

                await connection.ExecuteAsync(mergeSql, upsertItems);
                await transaction.CommitAsync();
            }
            catch (DbException)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
