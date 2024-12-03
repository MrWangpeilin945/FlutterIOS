
using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// 中止理由リポジトリ
/// </summary>
public class CancelReasonRepository : ICancelReasonRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    /// <summary>
    /// リポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    public CancelReasonRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    /// <summary>
    /// 中止理由を取得する
    /// </summary>
    public async Task<IEnumerable<Domain.Models.CancelReason>> GetCancelReasonsAsync()
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
             r.cancel_reason_id as CancelReasonId
             , r.name as Name
             , r.exam_item_id as ExamItemId 
         from
             resultcollector.cancel_reasons r 
         order by
             r.order_number;";

        var reasons = await connection.QueryAsync<Domain.Models.CancelReason>(sql);
        return reasons;
    }
}
