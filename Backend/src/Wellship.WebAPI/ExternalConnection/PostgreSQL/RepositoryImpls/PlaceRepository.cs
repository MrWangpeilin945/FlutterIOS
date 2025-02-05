using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.Utilities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Helper;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Transaction;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls
{
    /// <summary>
    /// 会場を登録するRepository層
    /// </summary>
    public class PlaceRepository : IPlaceRepository
    {
        private readonly IDbConnectionProvider _dbConnectionProvider;

        /// <summary>
        /// リポジトリを生成します。
        /// </summary>
        /// <param name="dbConnectionProvider">dbConnectionProvider</param>
        public PlaceRepository(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        /// <summary>
        /// 会場を登録するRepository層
        /// </summary>
        /// <param name="places">会場</param>
        /// <param name="createdAt">作成日時</param>
        /// <param name="createdBy">作成者</param>
        public async Task UpsertPlacesAsync(List<PlaceEntity> places, DateTimeOffset createdAt, string createdBy)
        {
            using var scope = TransactionScopeHelper.GetTransactionScope();
            {
                using var connection = await _dbConnectionProvider.GetOrOpenAsync();
                {
                    // 一時テーブル作成
                    string sqlCreateTempTable = @"
                        create temp table tmp_places(
                            place_code text not null
                            , name text not null
                        ) on commit drop;";
                    // 一時テーブル作成 SQL実行
                    await connection.ExecuteAsync(sqlCreateTempTable);

                    // 一時テーブルにINSERT
                    await BulkInsertHelper.BulkInsert(places,
                        place =>
                        $"(" +
                        $"{SqlFormatter.EscapeSqlValue(place.PlaceCode)}, " +
                        $"{SqlFormatter.EscapeSqlValue(place.Name)}" +
                        $")",
                        "tmp_places",
                        connection);


                    // UPSERT処理
                    string upsertSql = @"   
                        with max_order_number as (
                            select coalesce(max(order_number), 0) as max_number from resultcollector.places
                        )
                        insert into resultcollector.places (place_code, name, order_number, created_at, created_by)
                        select
                            place_code,
                            name,
                            max_order.max_number + row_number() over(),
                            @CreatedAt,
                            @CreatedBy
                        from tmp_places
                        cross join max_order_number as max_order
                        on conflict (place_code)
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
        /// 存在する会場コードを取得する。
        /// </summary>
        /// <param name="placeCodes">会場コードリスト</param>
        /// <returns>存在する会場コードリスト</returns>
        public async Task<List<string>> GetPlacesByCodesAsync(List<string> placeCodes)
        {
            var connection = await _dbConnectionProvider.GetOrOpenAsync();
            var sql = @"
                    select
                        place_code
                    from
                        resultcollector.places
                    where
                        place_code = any(@PlaceCodes);";

            var result = await connection.QueryAsync<string>(sql, new { PlaceCodes = placeCodes.ToArray() });
            return result.ToList();
        }

        /// <summary>
        /// 会場情報を取得する
        /// </summary>
        /// <param name="placeCodes">会場コード</param>
        /// <returns></returns>
        public async Task<List<PlaceEntity>> GetPlaceInfoAsync(List<string> placeCodes)
        {
            var connection = await _dbConnectionProvider.GetOrOpenAsync();
            var sql = @"
                    select
                        place_id as PlaceId, place_code as PlaceCode
                    from
                        resultcollector.places
                    where
                        place_code = any(@PlaceCodes);";

            var result = await connection.QueryAsync<PlaceEntity>(sql, new { PlaceCodes = placeCodes.ToArray() });

            return result.ToList();
        }

    }
}
