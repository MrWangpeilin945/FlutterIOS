using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// 検査結果リポジトリ
/// </summary>
public class ResultRepository : IResultRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;
    private readonly IStaffIdentityProvider _staffIdentityProvider;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// リポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    /// <param name="staffIdentityProvider">職員情報プロバイダ</param>
    /// <param name="timeProvider">timeProvider</param>
    public ResultRepository(IDbConnectionProvider dbConnectionProvider, IStaffIdentityProvider staffIdentityProvider, TimeProvider timeProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
        _staffIdentityProvider = staffIdentityProvider;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// 検査結果を登録する
    /// </summary>
    public async Task RegisterResultsAsync(Guid consultId, ExamResultRegisteEntity[] results)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();

        var operationTime = _timeProvider.GetUtcNow();
        var operationStaffCode = _staffIdentityProvider.StaffCode;
        if (operationStaffCode is null)
        {
            throw new WellshipAuthenticationException();
        }

        var param = results.Select(x => new
        {
            ConsultId = consultId,
            ExamItemDetailId = x.ExamItemDetailId,
            Value = x.Value,
            CreatedAt = operationTime,
            CreatedBy = operationStaffCode
        });

        // 検査結果を登録する
        const string mergeSql = @"
        merge 
        into resultcollector.exam_results as er 
            using (values (@ConsultId, @ExamItemDetailId, @Value, @CreatedAt, @CreatedBy)) as new_data( 
                consult_id
                , exam_item_detail_id
                , value
                , created_at
                , created_by
            ) 
                on er.consult_id = new_data.consult_id 
                and er.exam_item_detail_id = new_data.exam_item_detail_id when matched then update 
        set
            value = new_data.value 
            , created_at = new_data.created_at
            , created_by = new_data.created_by when not matched then 
        insert ( 
            consult_id
            , exam_item_detail_id
            , value
            , created_at
            , created_by
        ) 
        values ( 
            new_data.consult_id
            , new_data.exam_item_detail_id
            , new_data.value
            , new_data.created_at
            , new_data.created_by
        );";
        await connection.ExecuteAsync(mergeSql, param);
    }

    /// <summary>
    /// 検査結果登録時のログを記録する
    /// </summary>
    public async Task WriteResultsLogAsync(Guid consultId, ExamResultRegisteEntity[] results)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();

        var operationTime = _timeProvider.GetUtcNow();
        var operationStaffCode = _staffIdentityProvider.StaffCode;
        if (operationStaffCode is null)
        {
            throw new WellshipAuthenticationException();
        }

        var param = results.Select(x => new
        {
            ConsultId = consultId,
            ExamItemDetailId = x.ExamItemDetailId,
            Value = x.Value,
            CreatedAt = operationTime,
            CreatedBy = operationStaffCode
        });

        // 検査結果履歴を登録する
        const string sql = @"
        insert into resultcollector.exam_result_histories
        (
            consult_id
            , exam_item_detail_id
            , value
            , created_at
            , created_by
        )
        values
        (
            @ConsultId
            , @ExamItemDetailId
            , @Value
            , @CreatedAt
            , @CreatedBy
        );";

        await connection.ExecuteAsync(sql, param);
    }

    /// <summary>
    /// 受診を指定して複数の検査結果を取り消す
    /// </summary>
    /// <param name="consultId">受診ID</param>
    /// <param name="examItemDetailIds">削除対象の検査項目明細ID一覧</param>
    public async Task BatchDeleteResultsAsync(Guid consultId, int[] examItemDetailIds)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();

        var operationTime = _timeProvider.GetUtcNow();
        var operationStaffCode = _staffIdentityProvider.StaffCode;
        if (operationStaffCode is null)
        {
            throw new WellshipAuthenticationException();
        }

        var param = new
        {
            ConsultId = consultId,
            ExamItemDetailIds = examItemDetailIds,
            CreatedAt = operationTime,
            CreatedBy = operationStaffCode
        };

        // 検査結果削除履歴を登録して検査結果を削除する
        const string sql = @"
        begin; 
        
        -- 削除履歴テーブルにレコードを挿入
        insert 
        into resultcollector.exam_result_delete_histories( 
            consult_id
            , exam_item_detail_id
            , value
            , created_at
            , created_by
        ) 
        select
            consult_id
            , exam_item_detail_id
            , value
            , @CreatedAt
            , @CreatedBy
        from
            resultcollector.exam_results 
        where
            consult_id = @ConsultId
            and exam_item_detail_id = any (@ExamItemDetailIds); 
        
        -- 結果テーブルからレコードを削除
        delete 
        from
            resultcollector.exam_results 
        where
            consult_id = @ConsultId
            and exam_item_detail_id = any (@ExamItemDetailIds); 
        
        commit;";

        await connection.ExecuteAsync(sql, param);
    }

    /// <summary>
    /// 全ての検査結果を取得する
    /// </summary>
    /// <param name="consultNumber">受診番号</param>
    public async Task<IEnumerable<DisplayExamResultMenu>> GetConsultAllResults(string consultNumber)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
            select
                m.exam_menu_id                                          as ExamMenuId
                , m.name                                                as ExamMenuName
                , i.exam_item_id                                        as ExamItemId
                , i.name                                                as ExamItemName
                , d.exam_item_detail_id                                 as ExamItemDetailId
                , d.name                                                as ExamItemDetailName
                , case when d.type = 2 then op.name else r.value end    as CurrentResult
                , case when d.type = 2 then op.name else p.value end    as PastResult
                , p.exam_date                                           as PastDate
                , m.order_number                                        as MenuOrderNumber
                , i.order_number                                        as ItemOrderNumber
                , d.order_number                                        as ItemDetailOrderNumber
                , case when p.exam_date = (
                    select max(p2.exam_date)
                    from resultcollector.previous_results p2
                    where p2.exam_item_detail_id = p.exam_item_detail_id
                ) then true else false end                              as IsRecent -- 同じ検査項目明細IDの中で最も新しいものはtrue
            from
                resultcollector.exam_menus m
                left join resultcollector.exam_item_groups g
                    on m.exam_menu_id = g.exam_menu_id
                left join resultcollector.exam_items i
                    on g.exam_item_group_id = i.exam_item_group_id
                left join resultcollector.exam_item_details d
                    on i.exam_item_id = d.exam_item_id
                left join resultcollector.exam_item_detail_orders o
                    on d.exam_item_detail_id = o.exam_item_detail_id
                left join resultcollector.consult c
                    on o.consult_id = c.consult_id
                left join resultcollector.exam_item_detail_options op
                    on d.exam_item_detail_id = op.exam_item_detail_id
                left join resultcollector.exam_results r
                    on c.consult_id = r.consult_id
                    and d.exam_item_detail_id = r.exam_item_detail_id
                left join resultcollector.previous_results p
                    on c.consult_id = p.consult_id
                    and d.exam_item_detail_id = p.exam_item_detail_id
            where
                c.consult_number = @ConsultNumber
            order by
                m.order_number
                , i.order_number
                , d.order_number
                , p.exam_date desc";

        var consultAllResults = await connection.QueryAsync<ConsultAllResultEntity>(sql, new { ConsultNumber = consultNumber });

        var displayExamResultMenus = consultAllResults
            .GroupBy(result => new { result.ExamMenuId, result.ExamMenuName })
            .OrderBy(menuGroup => menuGroup.Key.ExamMenuId)
            .Select(menuGroup => new DisplayExamResultMenu
            {
                ExamMenuId = menuGroup.Key.ExamMenuId,
                ExamMenuName = menuGroup.Key.ExamMenuName,
                ExamItems = menuGroup.GroupBy(item => new { item.ExamItemId, item.ExamItemName })
                                     .OrderBy(itemGroup => itemGroup.Key.ExamItemId)
                                     .Select(itemGroup => new DisplayExamResultItem
                                     {
                                         ExamItemId = itemGroup.Key.ExamItemId,
                                         ExamItemName = itemGroup.Key.ExamItemName,
                                         ExamItemDetails = itemGroup.OrderBy(detail => detail.ItemDetailOrderNumber)
                                                                    .Select(detail => new DisplayExamResultItemDetail
                                                                    {
                                                                        ExamItemDetailId = detail.ExamItemDetailId,
                                                                        ExamItemDetailName = detail.ExamItemDetailName,
                                                                        CurrentResult = detail.CurrentResult,
                                                                        PastResult = detail.PastResult,
                                                                        PastDate = detail.PastDate,
                                                                        IsRecent = detail.IsRecent
                                                                    }).ToArray()
                                     }).ToArray()
            }).ToArray();

        return displayExamResultMenus;
    }
}
