using System.Data.Common;
using System.Transactions;
using Dapper;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Helper;
using Ryobi.Wellship.WebAPI.ExternalConnection.Utilities;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Transaction;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls
{
    /// <summary>
    /// 会場日程リポジトリ
    /// </summary>
    public class PlaceScheduleRepository : IPlaceScheduleRepository
    {
        private readonly IDbConnectionProvider _dbConnectionProvider;

        /// <summary>
        /// リポジトリを生成します。
        /// </summary>
        /// <param name="dbConnectionProvider">dbConnectionProvider</param>
        public PlaceScheduleRepository(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        /// <summary>
        /// 会場日程を登録する
        /// </summary>
        /// <param name="placeScheduleEntities"></param>
        /// <param name="createdAt"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        public async Task UpsertPlaceScheduleAsync(List<PlaceScheduleEntity> placeScheduleEntities, DateTime createdAt, string createdBy)
        {
            var connection = await _dbConnectionProvider.GetOrOpenAsync();
            var transaction = await connection.BeginTransactionAsync();
            try
            {
                var upsertItems = placeScheduleEntities.Select(p => new
                    {
                        PlaceId = p.PlaceId,
                        TeamId = p.TeamId,
                        Status = p.Status,
                        ExamDate = p.ExamDate,
                        StartTime = p.StartTime,
                        CreatedAt = createdAt,
                        CreatedBy = createdBy
                    }).ToArray();

                // Upsert文を実行
                    string mergeSql = @"
                        merge
                        into resultcollector.place_schedule as ps
                            using (values (@PlaceId, @TeamId, @Status, @ExamDate, @StartTime, @CreatedAt, @CreatedBy)) as new_data (
                                place_id,
                                team_id,
                                status,
                                exam_date,
                                start_time,
                                created_at,
                                created_by
                            )
                                on ps.place_id = new_data.place_id
                                and ps.team_id = new_data.team_id
                                and cast(ps.exam_date as date) = cast(new_data.exam_date as date) when matched then update
                        set
                            start_time = new_data.start_time,
                            created_at = new_data.created_at,
                            created_by = new_data.created_by when not matched then
                        insert (
                            place_id,
                            team_id,
                            status,
                            exam_date,
                            start_time,
                            created_at,
                            created_by
                        )
                        values (
                            new_data.place_id,
                            new_data.team_id,
                            new_data.status,
                            new_data.exam_date,
                            new_data.start_time,
                            new_data.created_at,
                            new_data.created_by
                        );";

                await connection.ExecuteAsync(mergeSql, upsertItems);
                await transaction.CommitAsync();
            }
            catch(DbException)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}
