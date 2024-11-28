
using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// 受診リポジトリ
/// </summary>
public class ConsultRepository : IConsultRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    /// <summary>
    /// 受診リポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    public ConsultRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    /// <summary>
    /// 受診が存在するか
    /// </summary>
    /// <param name="consultNumber">受診番号</param>
    public async Task<bool> ConsultExistsAsync(string consultNumber)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            count(1) 
        from
            resultcollector.consult 
        where
            consult_number = @ConsultNumber;";

        var results = await connection.QueryAsync<int>(sql, new { ConsultNumber = consultNumber });

        // 受診番号が一致するレコードが1件あればOK
        var consultExists = results.SingleOrDefault() == 1;
        return consultExists;
    }


    /// <summary>
    /// 未受診の検査項目明細を取得します。
    /// </summary>
    public async Task<IEnumerable<UnexaminedDetail>> GetUnexaminedDetailsAsync(string[] consultNumbers)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            c.consult_id
            , o.exam_item_detail_id 
        from
            resultcollector.consult c 
            inner join resultcollector.exam_item_detail_orders o 
                on c.consult_id = o.consult_id 
            left join resultcollector.exam_results r 
                on c.consult_id = r.consult_id 
                and o.exam_item_detail_id = r.exam_item_detail_id 
            left join resultcollector.exam_cancels ca 
                on c.consult_id = ca.consult_id 
                and o.exam_item_detail_id = ca.exam_item_detail_id 
        where
            o.consult_id = any (@ConsultNumbers) 
            and r.consult_id is null 
            and ca.consult_id is null 
        order by
            o.consult_id
            , o.exam_item_detail_id;";
        var unexaminedDetails = await connection.QueryAsync<UnexaminedDetailEntity>(sql, new { ConsultNumbers = consultNumbers });

        return unexaminedDetails.GroupBy(x => x.ConsultId)
                                .Select(g => new UnexaminedDetail()
                                {
                                    ConsultId = g.Key,
                                    ExamItemDetailIds = g.Select(d => d.ExamItemDetailId)
                                });
    }
}
