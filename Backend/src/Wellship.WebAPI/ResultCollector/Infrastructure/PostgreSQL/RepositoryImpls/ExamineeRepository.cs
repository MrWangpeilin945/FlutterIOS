
using Dapper;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// 受診者リポジトリ
/// </summary>
public class ExamineeRepository : IExamineeRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    /// <summary>
    /// リポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    public ExamineeRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    /// <summary>
    /// 受診者IDで受診者を取得します。
    /// </summary>
    /// <param name="examineeId">受診者ID</param>
    public async Task<Examinee> GetExamineeAsync(Guid examineeId)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            e.examinee_id as ExamineeId
            , e.examinee_code as ExamineeCode
            , e.name as Name
            , e.kana_name as KanaName
            , e.sex as Sex
            , e.birthdate as Birthdate 
            , a.organization_id as OrganizationId
            , o.organization_code as OrganizationCode
            , o.name as OrganizationName
            , o.order_number as OrderNumber
        from
            resultcollector.examinees e
            left join resultcollector.affiliations a
                on e.examinee_id = a.examinee_id
            left join resultcollector.organizations o
                on a.organization_id = o.organization_id
        where
            e.examinee_id = @ExamineeId;";

        var examinee = await connection.QueryAsync<ExamineeEntity>(sql, new { ExamineeId = examineeId });

        if (examinee is null)
        {
            throw new ResourceNotFoundException($"受診者が存在しません。ID: {examineeId}");
        }

        return new Examinee()
        {
            ExamineeId = examinee.First().ExamineeId,
            ExamineeCode = examinee.First().ExamineeCode,
            Name = examinee.First().Name,
            KanaName = examinee.First().KanaName,
            Sex = (Sex)examinee.First().Sex,
            Birthdate = new Birthdate(examinee.First().Birthdate),
            Affiliations = string.IsNullOrWhiteSpace(examinee.First().OrganizationCode) ? []
                            : examinee.Select(x => new Affiliations
                            {
                                OrganizationId = x.OrganizationId,
                                OrganizationCode = x.OrganizationCode,
                                OrganizationName = x.OrganizationName,
                                OrderNumber = x.OrderNumber
                            })
        };
    }

    /// <summary>
    /// 会場日程ID,検査メニューID,進捗状況に該当する受診ID一覧を取得します。
    /// </summary>
    /// <param name="placeScheduleId">会場日程ID</param>
    /// <param name="examMenuId">検査メニューID</param>
    /// <param name="status">進捗状況</param>
    public async Task<IEnumerable<Guid>> GetConsultIdsAsync(Guid placeScheduleId, int examMenuId, AggregatedProgressStatus status)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            consult_id as ConsultId
        from
            resultcollector.progress_exam_menus p
        where
            p.place_schedule_id = @PlaceScheduleId
            and p.exam_menu_id = @ExamMenuId
            and p.aggregated_status = @Status";

        var consultIds = await connection.QueryAsync<Guid>(sql, new { PlaceScheduleId = placeScheduleId, ExamMenuId = examMenuId , Status = (int)status });

        return consultIds;
    }

    /// <summary>
    /// 受診ID配列で受診者一覧を取得します。
    /// </summary>
    /// <param name="consultExamineeIds">受診者ID配列</param>
    public async Task<IEnumerable<ConsultExaminee>> GetConsultExamineesAsync(Guid[] consultExamineeIds)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            c.consult_number as ConsultNumber
            , t.ticket_number as TicketNumber
            , e.kana_name as KanaName
            , e.sex as Sex
            , t.created_at as CreatedAt
        from
            resultcollector.consult c
            left join resultcollector.tickets t
                on c.consult_id = t.consult_id
            left join resultcollector.examinees e
                on c.examinee_id = e.examinee_id
        where
            c.consult_id = any(@ConsultExamineeIds)
        order by
            c.consult_number";

        var examinees = await connection.QueryAsync<ConsultExamineeEntity>(sql, new { ConsultExamineeIds = consultExamineeIds });

        return examinees.Select(x => new ConsultExaminee()
        {
            ConsultNumber = x.ConsultNumber,
            TicketNumber = x.TicketNumber,
            KanaName = x.KanaName,
            Sex = (Sex)x.Sex,
            CreatedAt = x.CreatedAt
        });
    }
}
