
using Dapper;

using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// 進捗リポジトリ
/// </summary>
public class ProgressRepository : IProgressRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    /// <summary>
    /// リポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    public ProgressRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    /// <summary>
    /// 会場日程IDを指定して検査メニューごとの進捗状況を取得します。
    /// </summary>
    public async Task<AggregatedProgress> GetAggregatedProgressAsync(Guid placeScheduleId)
    {
        return await GetAggregatedProgress(placeScheduleId);
    }

    /// <summary>
    /// 会場日程IDと検査メニューIDを指定して対象検査メニューの進捗状況を取得します。
    /// </summary>
    public async Task<AggregatedProgressDetail> GetAggregatedProgressByMenuIdAsync(Guid placeScheduleId, int examMenuId)
    {
        var progress = await GetAggregatedProgress(placeScheduleId, examMenuId);
        var progressDetail = progress.AggregatedProgressDetails.SingleOrDefault(x => x.ExamMenuId == examMenuId);
        if (progressDetail is null) 
        {
            throw new ResourceNotFoundException();
        }
        return progressDetail;
    }

    private async Task<AggregatedProgress> GetAggregatedProgress(Guid placeScheduleId, int? examMenuId = null)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        var sql = @"
        select
            p.exam_menu_id as ExamMenuId
            , m.name as ExamMenuName
            , count( case when p.aggregated_status = 11 then 1 end ) as Count11
            , count( case when p.aggregated_status = 21 then 1 end ) as Count21
            , count( case when p.aggregated_status = 41 then 1 end ) as Count41
            , count( case when p.aggregated_status = 51 then 1 end ) as Count51
        from
            resultcollector.progress_exam_menus p
            inner join resultcollector.exam_menus m
                on p.exam_menu_id = m.exam_menu_id
        where
            p.place_schedule_id = @PlaceScheduleId";

        if (examMenuId is not null)
        {
            sql += @" and p.exam_menu_id = @ExamMenuId";
        }

        sql += @"
        group by
            p.exam_menu_id
            , m.name
            , m.order_number
        order by
            m.order_number;";

        var parameters = new { PlaceScheduleId = placeScheduleId, ExamMenuId = examMenuId };

        var response = await connection.QueryAsync<AggregatedProgressDetail>(sql, parameters);

        return new AggregatedProgress()
        {
            PlaceScheduleId = placeScheduleId,
            AggregatedProgressDetails = response
        };
    }
}
