using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Transaction;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;

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
                // 表示順を取得する
                const string selectOrderNumberSql = @"
                select 
                    coalesce(max(order_number), 0) + 1
                from 
                    resultcollector.places";
                var result = await connection.QueryAsync<int>(selectOrderNumberSql);
                int orderNumber = result.FirstOrDefault();

                    var upsertPlaces = places.Select((x, index) => new
                    {
                        PlaceId = Guid.NewGuid(),
                        PlaceCode = x.PlaceCode,
                        Name = x.Name,
                        OrderNumber = orderNumber + index,
                        CreatedAt = createdAt,
                        CreatedBy = createdBy,
                    }).ToArray();

                    // teams（班）
                    const string mergePlacesSql = @"
                    merge
                    into resultcollector.places as place
                        using (values (@PlaceId, @PlaceCode, @Name, @OrderNumber,
                                       @CreatedAt, @CreatedBy)) as new_data(
                            place_id
                            , place_code
                            , name
                            , order_number
                            , created_at
                            , created_by
                        )
                        on place.place_code = new_data.place_code
                    when matched then 
                        update set
                            name = new_data.name
                            , created_at = new_data.created_at
                            , created_by = new_data.created_by 
                    when not matched then
                        insert (
                            place_id
                            , place_code
                            , name
                            , order_number
                            , created_at
                            , created_by
                        )
                        values (
                            new_data.place_id
                            , new_data.place_code
                            , new_data.name
                            , new_data.order_number
                            , new_data.created_at
                            , new_data.created_by
                        );";
                    await connection.ExecuteAsync(mergePlacesSql, upsertPlaces);
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
