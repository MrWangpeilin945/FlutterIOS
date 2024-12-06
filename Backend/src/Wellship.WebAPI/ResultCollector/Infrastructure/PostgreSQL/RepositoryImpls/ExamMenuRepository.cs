
using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// 検査メニューリポジトリ
/// /// </summary>
public class ExamMenuRepository : IExamMenuRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    /// <summary>
    /// リポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    public ExamMenuRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    /// <summary>
    /// 検査メニューを取得します。
    /// テナントに設定されているすべての検査メニューを表示順昇順で取得します。 
    /// </summary>
    public async Task<IEnumerable<ExamMenu>> GetExamMenusAsync()
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            exam_menu_id as MenuId
            , name as MenuName 
        from
            resultcollector.exam_menus 
        order by
            order_number;";

        var results = await connection.QueryAsync<Domain.Models.ExamMenu>(sql);
        return results.ToList();
    }

    /// <summary>
    /// 前提検査メニューの設定一覧を取得します。
    /// </summary>
    public async Task<IEnumerable<PriorExamMenu>> GetPriorExamMenusAsync()
    {
        // TODO: SQLを書く
        return
        [
            new(2, [1]),
            new(3, [1,2,]),
            new(4, [1,2,3]),
            new(5, [1,2,3,4]),
            new(6, [1,2,3,4,5,]),
            new(7, [1,2,3,4,5,6,]),
            new(8, [1,2,3,4,5,6,7,]),
            new(9, [1,2,3,4,5,6,7,8]),
            new(10, [1,2,3,4,5,6,7,8,9])
        ];
    }
}
