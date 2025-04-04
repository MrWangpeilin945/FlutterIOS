using Dapper;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ExternalConnection.Enums;
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Transaction;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;

/// <summary>
/// 受診を更新するRepository層
/// </summary>
public class ConsultRepository : IConsultRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    /// <summary>
    /// リポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    public ConsultRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    /// <summary>
    /// 受診を更新する
    /// </summary>
    /// <param name="consults">更新する受診リスト</param>
    /// <param name="createdAt">作成日時</param>
    /// <param name="createdBy">作成者</param>
    public async Task UpsertConsultsAsync(List<ConsultEntity> consults, DateTimeOffset createdAt, string createdBy)
    {
        // ConnectionCodeでグループ化しSortNoの最大のレコードを絞り込む
        var actionConsults = consults.GroupBy(x => x.ConnectionCode,
                                             (y, z) => z.OrderByDescending(a => a.SortNo).First())
                                     .ToArray();
        using var scope = TransactionScopeHelper.GetTransactionScope();
        {
            using var connection = await _dbConnectionProvider.GetOrOpenAsync();
            {
                // 更新処理
                if (actionConsults.Any(x => x.ActionType == ActionType.登録))
                {
                    var upsertConsults = actionConsults.Where(x => x.ActionType == ActionType.登録)
                                                       .Select(x => new
                                                       {
                                                           ConsultId = Guid.NewGuid(),
                                                           ConsultNumber = x.ConsultNumber,
                                                           Age = x.Age,
                                                           ProgressStatus = (int)ConsultProgressStatus.来場待ち,
                                                           ExportStatus = (int)ConsultResultExportStatus.未出力,
                                                           PlaceScheduleId = x.PlaceScheduleId,
                                                           Note = x.Note,
                                                           ExamineeId = x.ExamineeId,
                                                           ExternalConnectionCode = x.ConnectionCode,
                                                           CreatedAt = createdAt,
                                                           CreatedBy = createdBy,
                                                           CancelStatus = (int)ConsultProgressStatus.キャンセル,
                                                           WaitStatus = (int)ConsultProgressStatus.来場待ち
                                                       }).ToArray();
                    // consult（受診）
                    const string mergeConsultSql = @"
                    merge
                    into resultcollector.consult as cs
                        using (values (@ConsultId, @ConsultNumber, @Age, @ProgressStatus, @ExportStatus, @PlaceScheduleId, @Note, 
                                       @ExamineeId, @ExternalConnectionCode, @CreatedAt, @CreatedBy)) as new_data(
                            consult_id
                            , consult_number
                            , age
                            , progress_status
                            , export_status
                            , place_schedule_id
                            , note
                            , examinee_id
                            , external_connection_code
                            , created_at
                            , created_by
                        )
                        on cs.external_connection_code = new_data.external_connection_code
                    when matched then 
                        update set
                            consult_number = new_data.consult_number
                            , age = new_data.age
                            , note = new_data.note
                            , external_connection_code = new_data.external_connection_code
                            , place_schedule_id = new_data.place_schedule_id
                            , examinee_id = new_data.examinee_id
                            , created_at = new_data.created_at
                            , created_by = new_data.created_by 
                            , progress_status =
                                CASE
                                    WHEN cs.progress_status = @CancelStatus THEN @WaitStatus
                                    ELSE cs.progress_status
                                END
                    when not matched then
                        insert (
                            consult_id
                            , consult_number
                            , age
                            , progress_status
                            , export_status
                            , place_schedule_id
                            , note
                            , examinee_id
                            , external_connection_code
                            , created_at
                            , created_by
                        )
                        values (
                            new_data.consult_id
                            , new_data.consult_number
                            , new_data.age
                            , new_data.progress_status
                            , new_data.export_status
                            , new_data.place_schedule_id
                            , new_data.note
                            , new_data.examinee_id
                            , new_data.external_connection_code
                            , new_data.created_at
                            , new_data.created_by
                        );";
                    await connection.ExecuteAsync(mergeConsultSql, upsertConsults);
                }

                // 削除処理
                if (actionConsults.Any(x => x.ActionType == ActionType.削除))
                {
                    // consult（受診）
                    var deleteConsults = actionConsults.Where(x => x.ActionType == ActionType.削除)
                                                       .Select(x => x.ConnectionCode)
                                                       .ToArray();
                    const string updateConsultSql = @"
                    update resultcollector.consult 
                    set
                        progress_status = @ProgressStatus
                        , note = ''
                        , created_at = @CreatedAt
                        , created_by = @CreatedBy
                    where
                        external_connection_code = any (@ConnectionCodes);";
                    await connection.ExecuteAsync(updateConsultSql, new
                    {
                        ProgressStatus = (int)ConsultProgressStatus.キャンセル,
                        CreatedAt = createdAt,
                        CreatedBy = createdBy,
                        ConnectionCodes = deleteConsults
                    });
                }

                var consultIds = actionConsults.Select(x => x.ConsultId).ToList();
                // consult_notes（受診特記）
                const string deleteConsultNotesSql = @"
                delete from 
                    resultcollector.consult_notes 
                where
                    consult_id = any (@ConsultIds);";
                await connection.ExecuteAsync(deleteConsultNotesSql, new { ConsultIds = consultIds });

                // external_exam_item_detail_orders（外部検査項目明細依頼）
                const string deleteExternalExamItemDetailOrdersSql = @"
                delete from
                    resultcollector.external_exam_item_detail_orders 
                where
                    consult_id = any (@ConsultIds);";
                await connection.ExecuteAsync(deleteExternalExamItemDetailOrdersSql, new { ConsultIds = consultIds });

                // exam_item_detail_orders（検査項目明細依頼）
                const string deleteExamItemDetailOrdersSql = @"
                delete from
                    resultcollector.exam_item_detail_orders 
                where
                    consult_id = any (@ConsultIds);";
                await connection.ExecuteAsync(deleteExamItemDetailOrdersSql, new { ConsultIds = consultIds });

                // consult_thresholds（基準値）
                const string deleteConsultThresholdsSql = @"
                delete from
                    resultcollector.consult_thresholds 
                where
                    consult_id = any (@ConsultIds);";
                await connection.ExecuteAsync(deleteConsultThresholdsSql, new { ConsultIds = consultIds });

                // previous_results（過去検査結果）
                const string deletePreviousResultsSql = @"
                delete from 
                    resultcollector.previous_results
                where
                    consult_id = any (@ConsultIds);";
                await connection.ExecuteAsync(deletePreviousResultsSql, new { ConsultIds = consultIds });

                if (actionConsults.Any(x => x.ActionType == ActionType.登録))
                {
                    // 外部連携キーから受診IDを取得
                    var consultConnectionCodes = actionConsults.Where(x => x.ActionType == ActionType.登録)
                                                               .Select(x => x.ConnectionCode).ToList();
                    const string selectConsultSQL = @"
                    select
                        consult_id as ConsultId
                        , external_connection_code as ExternalConnectionCode
                    from 
                        resultcollector.consult 
                    where 
                        external_connection_code = any (@ConnectionCodes);";
                    var consultExternalConnection = await connection.QueryAsync<ConsultExternalConnectionCodeEntity>(selectConsultSQL, new { ConnectionCodes = consultConnectionCodes });
                    var upsertConsults = actionConsults.Where(x => x.ActionType == ActionType.登録)
                                                       .Select(x => new ConsultEntity
                                                       {
                                                           ActionType = x.ActionType,
                                                           ConsultId = consultExternalConnection.Where(ex => ex.ExternalConnectionCode == x.ConnectionCode)
                                                                                                .Select(ex => ex.ConsultId).FirstOrDefault(),
                                                           ConsultNumber = x.ConsultNumber,
                                                           PlaceScheduleId = x.PlaceScheduleId,
                                                           Note = x.Note,
                                                           ExamineeId = x.ExamineeId,
                                                           Age = x.Age,
                                                           ConnectionCode = x.ConnectionCode,
                                                           SortNo = x.SortNo,
                                                           ConsultNotes = x.ConsultNotes,
                                                           ExamItemDetailOrders = x.ExamItemDetailOrders,
                                                           ConsultThresholds = x.ConsultThresholds,
                                                           PreviousResults = x.PreviousResults
                                                       });
                    // consult_notes（受診特記）
                    var consultNotes = upsertConsults.SelectMany(x => x.ConsultNotes.Select(cn => new
                    {
                        ConsultId = x.ConsultId,
                        Code = cn.Code,
                        Note = cn.Note,
                        CreatedAt = createdAt,
                        CreatedBy = createdBy
                    }));
                    const string insertConsultNotesSql = @"
                    insert into resultcollector.consult_notes 
                    (
                        consult_id
                        , code
                        , note
                        , created_at
                        , created_by
                    ) 
                    values
                    (
                        @ConsultId
                        , @Code
                        , @Note
                        , @CreatedAt
                        , @CreatedBy
                    );";
                    await connection.ExecuteAsync(insertConsultNotesSql, consultNotes);

                    // exam_item_detail_orders（検査項目明細依頼）
                    var examItemDetailOrders = upsertConsults
                        .SelectMany(x => x.ExamItemDetailOrders.Select(eo => new
                        {
                            ConsultId = x.ConsultId,
                            ExamItemDetailId = eo.ExamItemDetailId,
                            CreatedAt = createdAt,
                            CreatedBy = createdBy
                        }))
                        .GroupBy(order => new { order.ConsultId, order.ExamItemDetailId })
                        .Select(group => new
                        {
                            ConsultId = group.Key.ConsultId,
                            ExamItemDetailId = group.Key.ExamItemDetailId,
                            CreatedAt = group.First().CreatedAt,
                            CreatedBy = group.First().CreatedBy
                        });
                    const string insertExamItemDetailOrdersSql = @"
                    insert into resultcollector.exam_item_detail_orders 
                    (
                        consult_id
                        , exam_item_detail_id
                        , created_at
                        , created_by
                    ) 
                    values
                    (
                        @ConsultId
                        , @ExamItemDetailId
                        , @CreatedAt
                        , @CreatedBy
                    );";
                    await connection.ExecuteAsync(insertExamItemDetailOrdersSql, examItemDetailOrders);

                    // external_exam_item_detail_orders（外部検査項目明細依頼）
                    var externalExamItemDetailOrders = upsertConsults.SelectMany(x => x.ExamItemDetailOrders.Select(eo => new
                    {
                        ConsultId = x.ConsultId,
                        ExamItemDetailId = eo.ExamItemDetailId,
                        ExternalExamItemDetailCode = eo.ExamItemDetailCode,
                        ExternalNote = eo.Note,
                        CreatedAt = createdAt,
                        CreatedBy = createdBy
                    }));
                    const string insertExternalExamItemDetailOrdersSql = @"
                    insert into resultcollector.external_exam_item_detail_orders 
                    (
                        consult_id
                        , exam_item_detail_id
                        , external_exam_item_detail_code
                        , external_note
                        , created_at
                        , created_by
                    ) 
                    values
                    (
                        @ConsultId
                        , @ExamItemDetailId
                        , @ExternalExamItemDetailCode
                        , @ExternalNote
                        , @CreatedAt
                        , @CreatedBy
                    );";
                    await connection.ExecuteAsync(insertExternalExamItemDetailOrdersSql, externalExamItemDetailOrders);

                    // consult_thresholds（基準値）
                    var consultThresholds = upsertConsults.SelectMany(x => x.ConsultThresholds.Select(ct => new
                    {
                        ThresholdId = ct.ThresholdId,
                        ConsultId = x.ConsultId,
                        Priority = ct.Priority,
                        CreatedAt = createdAt,
                        CreatedBy = createdBy
                    }));
                    const string insertConsultThresholdsSql = @"
                    insert into resultcollector.consult_thresholds 
                    (
                        threshold_id
                        , consult_id
                        , priority
                        , created_at
                        , created_by
                    ) 
                    values
                    (
                        @ThresholdId
                        , @ConsultId
                        , @Priority
                        , @CreatedAt
                        , @CreatedBy
                    );";
                    await connection.ExecuteAsync(insertConsultThresholdsSql, consultThresholds);

                    // previous_results（過去検査結果）
                    var previousResults = upsertConsults.SelectMany(x => x.PreviousResults.Select(pr => new
                    {
                        ConsultId = x.ConsultId,
                        ExamDate = DateTime.Parse(pr.ExamDate.ToString()),
                        ExamItemDetailId = pr.ExamItemDetailId,
                        Value = pr.Value,
                        CreatedAt = createdAt,
                        CreatedBy = createdBy
                    }));
                    const string insertPreviousResultsSql = @"
                    insert into resultcollector.previous_results 
                    (
                        consult_id
                        , exam_date
                        , exam_item_detail_id
                        , value
                        , created_at
                        , created_by
                    ) 
                    values
                    (
                        @ConsultId
                        , @ExamDate
                        , @ExamItemDetailId
                        , @Value
                        , @CreatedAt
                        , @CreatedBy
                    );";
                    await connection.ExecuteAsync(insertPreviousResultsSql, previousResults);
                }
                scope.Complete();
            }
        }
    }

    /// <summary>
    /// 存在する検査メニュー特記コード情報（検査特記コード、検査特記名）を取得する
    /// </summary>
    /// <param name="codes">検査メニュー特記コードのリスト</param>
    public async Task<List<ExamMenuNoteCodeEntity>> GetExamMenuNodeCodeInfoAsync(List<string> codes)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        var sql = @"
                select
                    code as Code
                    , name as Name
                from
                    resultcollector.exam_menu_note_codes
                where
                    code = any(@Codes);";

        var result = await connection.QueryAsync<ExamMenuNoteCodeEntity>(sql, new { Codes = codes });
        return result.ToList();
    }

    /// <summary>
    /// 存在する外部検査項目明細情報（検査項目明細ID、外部コード検査項目明細CD）を取得する
    /// </summary>
    /// <param name="codes">外部コード検査項目明細コードのリスト</param>
    public async Task<List<ExternalExamItemDetailEntity>> GetExternalExamItemDetailInfoAsync(List<string> codes)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        var sql = @"
                select
                    exam_item_detail_id as ExamItemDetailId
                    , external_exam_item_detail_code as ExternalExamItemDetailCode
                from
                    resultcollector.external_exam_item_details
                where
                    external_exam_item_detail_code = any(@Codes);";

        var result = await connection.QueryAsync<ExternalExamItemDetailEntity>(sql, new { Codes = codes });
        return result.ToList();
    }

    /// <summary>
    /// 存在する受診情報の外部連携キーを取得する
    /// </summary>
    /// <param name="codes">連携キーのリスト</param>
    public async Task<List<ExternalConnectionCodeEntity>> GetExternalConnectionCodeAsync(List<string> codes)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        var sql = @"
                select
                    consult_id as ConsultId
                    , consult_number as ConsultNumber
                    , external_connection_code as ConnectionCode
                from
                    resultcollector.consult
                where
                    external_connection_code = any(@Codes);";

        var result = await connection.QueryAsync<ExternalConnectionCodeEntity>(sql, new { Codes = codes });
        return result.ToList();
    }

    /// <summary>
    /// 受診番号に紐づけられた外部連携キーを取得する
    /// </summary>
    /// <param name="consultNumbers">受診番号のリスト</param>
    public async Task<List<ExternalConnectionCodeEntity>> GetConsultExternalConnectionCodeAsync(List<string> consultNumbers)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        var sql = @"
                select
                    consult_id as ConsultId
                    , consult_number as ConsultNumber
                    , external_connection_code as ConnectionCode
                from
                    resultcollector.consult
                where
                    consult_number = any(@ConsultNumbers);";

        var result = await connection.QueryAsync<ExternalConnectionCodeEntity>(sql, new { ConsultNumbers = consultNumbers });
        return result.ToList();
    }

}
