using Dapper;

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
}
