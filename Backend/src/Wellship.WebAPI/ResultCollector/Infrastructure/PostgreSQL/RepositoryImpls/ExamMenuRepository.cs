
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
    /// 現在の検査メニューIDを指定して、前提検査メニューの設定一覧を取得します。
    /// 設定がなければnullを返します。 
    /// </summary>
    public async Task<PriorExamMenu?> GetPriorExamMenusAsync(int currentExamMenuId)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            current_exam_menu_id as CurrentExamMenuId
            , prior_exam_menu_id as PriorExamMenuId 
        from
            resultcollector.prior_exam_menus
        where
            current_exam_menu_id = @CurrentExamMenuId;";

        var results = await connection.QueryAsync<Entities.PriorExamMenuEntity>(sql, new { CurrentExamMenuId = currentExamMenuId });

        if (!results.Any())
        {
            return null;
        }

        var priorMenuIds = results.Select(x => x.PriorExamMenuId);
        return new PriorExamMenu(currentExamMenuId, priorMenuIds);
    }
}
