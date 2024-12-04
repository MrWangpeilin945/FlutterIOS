
using Dapper;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

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
    /// 受診を取得します。
    /// </summary>
    public async Task<Consult> GetConsultAsync(string consultNumber)
    {
        var results = await GetConsultsAsync([consultNumber]);
        if (!results.Any())
        {
            throw new ConsultNumberNotFoundException();
        }

        return results.Single();
    }

    /// <summary>
    /// 受診リストを取得します。
    /// </summary>
    public async Task<IEnumerable<Consult>> GetConsultsAsync(string[] consultNumbers)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            consult_id as ConsultId
            , consult_number as ConsultNumber
            , progress_status as ProgressStatus
            , export_status as ExportStatus
            , place_schedule_id as PlaceScheduleId
            , examinee_id as ExamineeId 
        from
            resultcollector.consult 
        where
            consult_number = any(@ConsultNumbers)
        order by
            consult_id;";

        var consults = await connection.QueryAsync<ConsultEntity>(sql, new { ConsultNumbers = consultNumbers });

        return consults.Select(x => new Consult()
        {
            ConsultId = x.ConsultId,
            ConsultNumber = x.ConsultNumber,
            ExamineeId = x.ExamineeId,
            PlaceScheduleId = x.PlaceScheduleId,
            ExportStatus = (ConsultResultExportStatus)x.ExportStatus,
            ProgressStatus = (ConsultProgressStatus)x.ProgressStatus,
        });
    }

    /// <summary>
    /// 受診番号を指定して未受診の検査項目を取得します。
    /// </summary>
    public async Task<UnexaminedConsult> GetUnexaminedConsultAsync(string consultNumber)
    {
        var results = await GetUnexaminedConsultsAsync([consultNumber]);
        if (!results.Any())
        {
            throw new ConsultNumberNotFoundException();
        }

        return results.Single();
    }

    /// <summary>
    /// 未受診の検査項目明細を受診単位のリストで取得します。
    /// </summary>
    public async Task<IEnumerable<UnexaminedConsult>> GetUnexaminedConsultsAsync(string[] consultNumbers)
    {
        // NOTE: 検査項目明細単位の依頼に対して、検査結果あるいは検査中止のレコードが存在すれば受診済みとする
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            c.consult_id as ConsultId
            , c.consult_number as ConsultNumber
            , c.examinee_id as ExamineeId
            , m.exam_menu_id as ExamMenuId
            , m.name as ExamMenuName 
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
            left join resultcollector.exam_item_details d 
                on o.exam_item_detail_id = d.exam_item_detail_id 
            left join resultcollector.exam_items i 
                on d.exam_item_id = i.exam_item_id 
            left join resultcollector.exam_item_groups g 
                on i.exam_item_group_id = g.exam_item_group_id 
            left join resultcollector.exam_menus m 
                on g.exam_menu_id = m.exam_menu_id 
        where
            c.consult_number = any (@ConsultNumbers)
            and r.consult_id is null 
            and ca.consult_id is null 
        order by
            o.consult_id
            , m.order_number;";

        var unexaminedDetails = await connection.QueryAsync<UnexaminedMenuEntity>(sql, new { ConsultNumbers = consultNumbers });

        var unexaminedConsults = unexaminedDetails
                                 .GroupBy(d => d.ConsultId)
                                 .Select(g => new UnexaminedConsult
                                 {
                                     ConsultId = g.Key,
                                     ExamineeId = g.First().ExamineeId,
                                     ConsultNumber = g.First().ConsultNumber,
                                     UnexaminedExamMenus = g.GroupBy(d => d.ExamMenuId)
                                                            .Select(gi => new UnexaminedExamMenu
                                                            {
                                                                ExamMenuId = gi.Key,
                                                                ExamMenuName = gi.First().ExamMenuName,
                                                            })
                                 });
        return unexaminedConsults;
    }

    /// <summary>
    /// 受診を指定して検査中止を取得します。
    /// </summary>
    public async Task<ExamCancel> GetExamCancelsAsync(int consultId)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            c.consult_id as ConsultId
            , d.exam_item_id as ExamItemId
            , c.exam_item_detail_id as ExamItemDetailId
            , c.cancel_reason_id as CancelReasonId 
        from
            resultcollector.exam_cancels c 
            left join resultcollector.exam_item_details d 
                on c.exam_item_detail_id = d.exam_item_detail_id
        where
            c.consult_id = @ConsultId;";

        var response = await connection.QueryAsync<ExamCancelEntity>(sql, new { ConsultId = consultId });

        return new ExamCancel()
        {
            ConsultId = consultId,
            ExamItemDetailCancels = response.Select(x => new ExamItemDetailCancel()
            {
                ExamItemId = x.ExamItemId,
                ExamItemDetailId = x.ExamItemDetailId,
                CancelReasonId = x.CancelReasonId
            })
        };
    }

    /// <summary>
    /// 検査中止を削除します。
    /// </summary>
    public async Task RemoveExamCancelsAsync(int consultId, int[] examItemDetailIds)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        delete 
        from
            resultcollector.exam_cancels 
        where
            consult_id = @ConsultId
            and exam_item_detail_id = any (@ExamItemDetailIds);";

        await connection.ExecuteAsync(sql, new { ConsultId = consultId, ExamItemDetailIds = examItemDetailIds });
    }

    /// <summary>
    /// 検査中止を保存します。
    /// すでに同じ検査項目明細の中止が存在すれば上書き更新、存在しなければ新規作成します。
    /// </summary>
    public async Task SaveExamCancelsAsync(int consultId, IEnumerable<ExamItemCancel> examItemCancels)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();

        var operationTime = DateTime.UtcNow;

        const string preSelectSql = @"
        select
            d.exam_item_id as ExamItemId
            , od.exam_item_detail_id as ExamItemDetailId
        from
            resultcollector.exam_item_detail_orders od 
            left join resultcollector.exam_item_details d 
                on od.exam_item_detail_id = d.exam_item_detail_id 
        where
            exam_item_id = any (@ExamItemIds) 
            and od.consult_id = @ConsultId;";

        // マスタから検査項目ー検査項目明細の関連付けを取得する（検査依頼があるものに絞り込む）
        var examItemIds = examItemCancels.Select(x => x.ExamItemId).Distinct().ToArray();
        var orderedItemDetails = await connection.QueryAsync<(int examItemId, int examItemDetailId)>(preSelectSql, new { ConsultId = consultId, ExamItemIds = examItemIds });

        // 検査項目明細単位で保存するオブジェクトを作る
        var saveItems = new List<object>();
        foreach (var examItemCancel in examItemCancels)
        {
            // 明細単位にばらす（依頼ありのみ）
            var detailIds = orderedItemDetails.Where(x => x.examItemId == examItemCancel.ExamItemId).Select(x => x.examItemDetailId).Distinct().ToArray();
            foreach (var detailId in detailIds)
            {
                var item = new
                {
                    ConsultId = consultId,
                    ExamItemDetailId = detailId,
                    CancelReasonId = examItemCancel.CancelReasonId,
                    CreatedAt = operationTime, // TODO: システム時刻を取るサービスを作る 
                    CreatedBy = "作成者（仮）" // TODO: JWTから操作者の情報を取得する
                };
                saveItems.Add(item);
            }
        }

        // UPSERTを実行する
        // 複合主キーのconsult_idとexam_item_detail_idが一致するレコードがあれば
        // cancel_reason_id, created_at, created_byを更新する
        // なければレコードを新規作成する
        const string mergeSql = @"
        merge 
        into resultcollector.exam_cancels as ec 
            using (values (@ConsultId, @ExamItemDetailId, @CancelReasonId, @CreatedAt, @CreatedBy)) as new_data( 
                consult_id
                , exam_item_detail_id
                , cancel_reason_id
                , created_at
                , created_by
            ) 
                on ec.consult_id = new_data.consult_id 
                and ec.exam_item_detail_id = new_data.exam_item_detail_id when matched then update 
        set
            cancel_reason_id = new_data.cancel_reason_id 
            , created_at = new_data.created_at
            , created_by = new_data.created_by when not matched then 
        insert ( 
            consult_id
            , exam_item_detail_id
            , cancel_reason_id
            , created_at
            , created_by
        ) 
        values ( 
            new_data.consult_id
            , new_data.exam_item_detail_id
            , new_data.cancel_reason_id
            , new_data.created_at
            , new_data.created_by
        );";
        await connection.ExecuteAsync(mergeSql, saveItems);
    }
}
