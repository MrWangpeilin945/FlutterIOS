
using Dapper;

using Ryobi.Wellship.Core.Enums;
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

    /// <summary>
    /// 検査メニュー特記の設定一覧を取得します。
    /// </summary>
    public async Task<IEnumerable<MenuNote>> GetMenuNotesAsync(int examMenuId)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();

        // メインテーブル
        const string mainSql = @"
        select
            n.menu_note_id as MenuNoteId
            , n.name as Name
            , n.order_number as OrderNumber
            , n.exam_menu_id as ExamMenuId 
        from
            resultcollector.exam_menu_notes n 
        where
            n.exam_menu_id = @ExamMenuId 
        order by
            n.order_number;";

        // 検査メニュー特記_検査項目テーブル
        const string itemsSql = @"
        select
            n.menu_note_id
            , nr.exam_item_detail_id
            , nr.source_type
            , nr.order_number 
        from
            resultcollector.exam_menu_notes n 
            left join resultcollector.exam_menu_note_results nr 
                on nr.menu_note_id = n.menu_note_id 
        where
            n.exam_menu_id = @ExamMenuId 
        order by
            n.order_number
            , nr.order_number;";


        // 検査メニュー特記_検査結果テーブル
        const string resultsSql = @"
        select
            n.menu_note_id as MenuNoteId
            , n.name as Name
            , n.order_number as OrderNumber
            , n.exam_menu_id as ExamMenuId 
        from
            resultcollector.exam_menu_notes n 
        where
            n.exam_menu_id = @ExamMenuId 
        order by
            n.order_number;";

        // SQL実行
        var menuNotes = await connection.QueryAsync<Entities.MenuNoteEntity>(mainSql, new { ExamMenuId = examMenuId });
        var noteExamItems = await connection.QueryAsync<Entities.MenuNoteExamItemEntity>(itemsSql, new { ExamMenuId = examMenuId });
        var noteResults = await connection.QueryAsync<Entities.MenuNoteExamResultEntity>(resultsSql, new { ExamMenuId = examMenuId });

        // ドメインモデルにマッピング
        return menuNotes.OrderBy(x => x.OrderNumber)
                        .Select(x => new MenuNote()
                        {
                            MenuNoteId = x.MenuNoteId,
                            Name = x.Name,
                            ExamMenuId = x.ExamMenuId,
                            ExamItemNotes = noteExamItems.Where(n => n.MenuNoteId == x.MenuNoteId)
                                                         .OrderBy(n => n.OrderNumber)
                                                         .Select(n => new MenuNoteExamItem()
                                                         {
                                                             ExamItemId = n.ExamItemId
                                                         }),
                            ExamResults = noteResults.Where(n => n.MenuNoteId == x.MenuNoteId)
                                                     .OrderBy(n => n.OrderNumber)
                                                     .Select(n => new MenuNoteExamResult()
                                                     {
                                                         SourceType = (SourceType)n.SourceType,
                                                         ExamItemDetailId = n.ExamItemDetailId
                                                     })
                        });
    }
}
