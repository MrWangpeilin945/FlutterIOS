using Dapper;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// 会場日程リポジトリ
/// </summary>
public class PlaceScheduleRepository : IPlaceScheduleRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PlaceScheduleRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
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
            ExamDate = new DateOnly(),
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
}
