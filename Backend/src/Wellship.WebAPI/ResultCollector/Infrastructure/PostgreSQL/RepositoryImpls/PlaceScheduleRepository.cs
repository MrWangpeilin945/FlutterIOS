using Dapper;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// 会場日程リポジトリ
/// </summary>
public class PlaceScheduleRepository : IPlaceScheduleRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;
    private readonly IStaffIdentityProvider _staffIdentityProvider;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PlaceScheduleRepository(IDbConnectionProvider dbConnectionProvider, IStaffIdentityProvider staffIdentityProvider, TimeProvider timeProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
        _staffIdentityProvider = staffIdentityProvider;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// 健診日を指定して会場日程を取得する
    /// </summary>
    public async Task<IEnumerable<Domain.Models.PlaceSchedule>> GetPlaceSchedulesAsync(DateOnly examDate)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            ps.place_schedule_id as PlaceScheduleId
            , ps.status as Status
            , ps.exam_date as ExamDate
            , ps.start_time as StartTime
            , p.place_id as PlaceId
            , p.place_code as PlaceCode
            , p.name as PlaceName
            , p.order_number as PlaceOrderNumber
            , t.team_id as TeamId
            , t.team_code as TeamCode
            , t.name as TeamName
            , t.order_number as TeamOrderNumber 
        from
            resultcollector.place_schedule ps 
            left join resultcollector.places p 
                on ps.place_id = p.place_id 
            left join resultcollector.teams t 
                on ps.team_id = t.team_id
        where
            ps.exam_date = @ExamDate::date
        order by StartTime asc;";

        // DapperはDateOnlyを扱えないため、パラメータは文字列として渡しています。
        var response = await connection.QueryAsync<PlaceSchedulePlaceTeam>(sql, new { ExamDate = examDate.ToString("yyyy-MM-dd") });

        // 平たく取得したものをアプリ内部で扱いやすいようにドメインモデルに変換して返します。
        return response.Select(x => new Domain.Models.PlaceSchedule()
        {
            Id = x.PlaceScheduleId,
            ExamDate = DateOnly.FromDateTime(x.ExamDate),
            StartTime = x.StartTime,
            PlaceScheduleLockingStatus = (PlaceScheduleLockingStatus)x.Status,
            Place = new Domain.Models.Place()
            {
                Id = x.PlaceId,
                Code = x.PlaceCode,
                Name = x.PlaceName,
                OrderNumber = x.PlaceOrderNumber
            },
            Team = new Domain.Models.Team()
            {
                Id = x.TeamId,
                Code = x.TeamCode,
                Name = x.TeamName,
                OrderNumber = x.TeamOrderNumber
            },
        }).ToArray();
    }

    /// <summary>
    /// 会場日程IDを指定して会場日程リストを取得する
    /// </summary>
    public async Task<Domain.Models.PlaceSchedule> GetPlaceScheduleAsync(Guid placeScheduleId)
    {
        var placeSchedule = await GetPlaceSchedulesAsync([placeScheduleId]);
        if (!placeSchedule.Any())
        {
            // 会場日程が存在しない            
            throw new PlaceScheduleNotFoundException();
        }
        return placeSchedule.ElementAt(0);
    }

    /// <summary>
    /// 会場日程IDを指定して会場日程を取得する
    /// </summary>
    public async Task<IEnumerable<Domain.Models.PlaceSchedule>> GetPlaceSchedulesAsync(Guid[] placeScheduleIds)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            ps.place_schedule_id as PlaceScheduleId
            , ps.status as Status
            , ps.exam_date as ExamDate
            , ps.start_time as StartTime
            , p.place_id as PlaceId
            , p.place_code as PlaceCode
            , p.name as PlaceName
            , p.order_number as PlaceOrderNumber
            , t.team_id as TeamId
            , t.team_code as TeamCode
            , t.name as TeamName
            , t.order_number as TeamOrderNumber 
        from
            resultcollector.place_schedule ps 
            left join resultcollector.places p 
                on ps.place_id = p.place_id 
            left join resultcollector.teams t 
                on ps.team_id = t.team_id
        where
            ps.place_schedule_id = any(@PlaceScheduleIds)
        order by StartTime asc;";

        var response = await connection.QueryAsync<PlaceSchedulePlaceTeam>(sql, new { PlaceScheduleIds = placeScheduleIds });

        return response.Select(x => new Domain.Models.PlaceSchedule()
        {
            Id = x.PlaceScheduleId,
            ExamDate = DateOnly.FromDateTime(x.ExamDate),
            StartTime = x.StartTime,
            PlaceScheduleLockingStatus = (PlaceScheduleLockingStatus)x.Status,
            Place = new Domain.Models.Place()
            {
                Id = x.PlaceId,
                Code = x.PlaceCode,
                Name = x.PlaceName,
                OrderNumber = x.PlaceOrderNumber
            },
            Team = new Domain.Models.Team()
            {
                Id = x.TeamId,
                Code = x.TeamCode,
                Name = x.TeamName,
                OrderNumber = x.TeamOrderNumber
            },
        }).ToArray();
    }

    /// <summary>
    /// 会場ロック状態を取得する
    /// </summary>
    public async Task<Domain.Models.PlaceScheduleStatus> GetPlaceScheduleLockingStatusAsync(Guid placeScheduleId)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            ps.place_schedule_id as PlaceScheduleId
            , ps.place_id as PlaceId
            , p.name as PlaceName
            , ps.exam_date as ExamDate
            , ps.status as Status
            , ps.created_at as CreatedAt
            , ps.created_by as CreatedBy
        from
            resultcollector.place_schedule as ps
            left join resultcollector.places as p 
                on ps.place_id = p.place_id 
        where
            ps.place_schedule_id = @PlaceScheduleId;";

        var results = await connection.QueryAsync<Domain.Models.PlaceScheduleStatus>(sql, new { PlaceScheduleId = placeScheduleId });

        var placeSchedule = results.SingleOrDefault();

        if (placeSchedule is null)
        {
            throw new PlaceScheduleNotFoundException();
        }
        return placeSchedule;
    }

    /// <summary>
    /// 会場ロック状態を更新する
    /// </summary>
    public async Task UpdatePlaceScheduleLockingStatusAsync(Guid placeScheduleId, PlaceScheduleLockingStatus status)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string selectSql = @"
        select
            ps.place_schedule_id as PlaceScheduleId
            , ps.status as Status
        from
            resultcollector.place_schedule as ps
        where
            ps.place_schedule_id = @PlaceScheduleId;";

        var results = await connection.QueryAsync<Domain.Models.PlaceScheduleStatus>(selectSql, new { PlaceScheduleId = placeScheduleId });

        var placeSchedule = results.SingleOrDefault();

        if (placeSchedule is null)
        {
            throw new PlaceScheduleNotFoundException();
        }
        const string updateSql = @"
        update resultcollector.place_schedule 
        set
            status = @Status,
            created_by = @CreatedBy,
            created_at = @CreatedAt
        where
            place_schedule_id = @PlaceScheduleId;";

        var param = new
        {
            Status = (int)status,
            CreatedBy = _staffIdentityProvider.StaffCode,
            CreatedAt = _timeProvider.GetUtcNow(),
            PlaceScheduleId = placeScheduleId
        };

        await connection.QueryAsync(updateSql, param);
    }


    /// <summary>
    /// 会場ロックの履歴を記録する
    /// </summary>
    public async Task WriteLockLogAsync(Guid placeScheduleId, PlaceScheduleLockingStatus status)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();

        const string sql = @"
        insert 
        into resultcollector.place_schedule_lock_histoies( 
            place_schedule_id
            , status
            , created_at
            , created_by
        ) 
        values ( 
            @PlaceScheduleId
            , @Status
            , @CreatedBy
            , @CreatedAt
        );";

        var param = new
        {
            PlaceScheduleId = placeScheduleId,
            Status = (int)status,
            CreatedBy = _staffIdentityProvider.StaffCode,
            CreatedAt = _timeProvider.GetUtcNow()
        };

        await connection.QueryAsync(sql, param);
    }

    /// <summary>
    /// 会場日程での同姓同名の有無を判定する
    /// </summary>
    public async Task<bool> IsSamenameAsync(string consultNumber)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            count(*)
        from
            resultcollector.consult as c
            left join resultcollector.examinees as e 
                on c.examinee_id = e.examinee_id 
        where
            e.examinee_id != (
                select 
                    examinee_id 
                from 
                    resultcollector.consult 
                where 
                    consult_number = @ConsultNumber
                )
            and e.kana_name = (
                select 
                    e2.kana_name
                from 
                    resultcollector.consult c2
                    left join resultcollector.examinees e2
                        on c2.examinee_id = e2.examinee_id
                where 
                    c2.consult_number = @ConsultNumber
            )
            and c.place_schedule_id = (
                select
                    place_schedule_id
                from
                    resultcollector.consult
                where
                    consult_number = @ConsultNumber
            );";
        var results = await connection.QueryAsync<int>(sql, new { ConsultNumber = consultNumber });
        return results.Any(x => x != 0);
    }
}
