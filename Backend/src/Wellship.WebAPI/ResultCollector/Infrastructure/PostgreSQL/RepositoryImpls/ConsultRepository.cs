
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
            , d.exam_item_id as ExamItemId
            , o.exam_item_detail_id as ExamItemDetailId
            , i.name as ExamItemName
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
        where
            c.consult_number = any (@ConsultNumbers)
            and r.consult_id is null 
            and ca.consult_id is null 
        order by
            o.consult_id
            , d.exam_item_id
            , o.exam_item_detail_id;";

        var unexaminedDetails = await connection.QueryAsync<UnexaminedDetailEntity>(sql, new { ConsultNumbers = consultNumbers });

        var unexaminedConsults = unexaminedDetails
                                 .GroupBy(d => d.ConsultId)
                                 .Select(g => new UnexaminedConsult
                                 {
                                     ConsultId = g.Key,
                                     ExamineeId = g.First().ExamineeId,
                                     ConsultNumber = g.First().ConsultNumber,
                                     UnexaminedExamItems = g.GroupBy(d => d.ExamItemId)
                                                            .Select(gi => new UnexaminedExamItem
                                                            {
                                                                ExamItemId = gi.Key,
                                                                ExamItemName = gi.First().ExamItemName,
                                                                ExamItemDetailIds = gi.Select(d => d.ExamItemDetailId)
                                                            })
                                 });
        return unexaminedConsults;
    }
}
