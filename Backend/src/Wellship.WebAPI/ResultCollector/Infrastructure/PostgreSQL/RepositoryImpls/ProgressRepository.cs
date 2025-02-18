
using Dapper;

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
    /// 会場日程IDを指定して検査項目ごとの進捗状況を取得します。
    /// </summary>
    public async Task<AggregatedProgress> GetAggregatedProgressAsync(Guid placeScheduleId)
    {
        // NOTE: 進捗の算出方法
        // 検査項目明細単位の依頼に対して
        //  - 結果レコードあり => 検査済み
        //  - 中止レコードあり => 検査中止
        //  - 上記のレコードなし => 未実施
        // 
        // 検査項目単位にグルーピングして
        //  - 未実施が1つでもあれば => 未実施
        //  - すべて検査中止であれば => 検査中止
        //  - 上記以外で検査済みを含む => 検査済み
        // 
        // 検査項目単位のステータスと受診のステータスの組み合わせで集計用の進捗を計算する
        //  - [検査項目]未実施11 x [受診]来場待ち11 => 予定11
        //  - [検査項目]未実施11 x [受診]検査中21 => 来場21
        //  - [検査項目]検査済み41 x [受診]検査中21 => 済41
        //  - [検査項目]検査中止51 x [受診]検査中21 => 中止51

        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        with 明細結果 as ( 
            select
                c.consult_id
                , d.exam_item_id
                , o.exam_item_detail_id
                , case 
                    when r.exam_item_detail_id is not null -- 結果レコードがあれば実施済み
                        then 41 
                    when ca.exam_item_detail_id is not null -- 中止レコードがあれば検査中止
                        then 51 
                    else 11                             -- レコードがなければ未実施
                    end as detail_status 
            from
                resultcollector.consult c 
                inner join resultcollector.exam_item_detail_orders o 
                    on c.consult_id = o.consult_id 
                left join resultcollector.exam_results r 
                    on o.exam_item_detail_id = r.exam_item_detail_id 
                    and o.consult_id = r.consult_id 
                left join resultcollector.exam_cancels ca 
                    on o.exam_item_detail_id = ca.exam_item_detail_id 
                    and o.consult_id = ca.consult_id 
                left join resultcollector.exam_item_details d 
                    on o.exam_item_detail_id = d.exam_item_detail_id 
            where
                c.progress_status not in (41)           -- 受診の進捗状況がキャンセル:41は除外する
                and c.place_schedule_id = @PlaceScheduleId
        ) 
        , 受診検査項目 as ( 
            select
                明細結果.consult_id
                , 明細結果.exam_item_id
                , case 
                    when 11 = any (ARRAY_AGG(明細結果.detail_status)) -- 未実施が一つでもあれば未実施
                        then 11 
                    when 51 = all (ARRAY_AGG(明細結果.detail_status)) -- すべて検査中止であれば検査中止
                        then 51 
                    when 41 = any (ARRAY_AGG(明細結果.detail_status)) -- 上記以外で検査済みを含む場合は検査済み
                        then 41 
                    end as item_status 
            from
                明細結果 
            group by
                consult_id
                , exam_item_id
        ) 
        , 集計結果 as ( 
            select
                受診検査項目.consult_id
                , 受診検査項目.exam_item_id
                , 受診検査項目.item_status
                , con.progress_status as consult_status
                , case 
                    when 受診検査項目.item_status = 11 
                    and con.progress_status = 11 
                        then 11 
                    when 受診検査項目.item_status = 11 
                    and con.progress_status = 21 
                        then 21 
                    when 受診検査項目.item_status = 41 
                    and con.progress_status = 21 
                        then 41 
                    when 受診検査項目.item_status = 51 
                    and con.progress_status = 21 
                        then 51 
                    end as 集計ステータス 
            from
                受診検査項目 
                left join resultcollector.consult con 
                    on 受診検査項目.consult_id = con.consult_id
        ) 
        , 検査項目集計 as ( 
            select
                集計結果.exam_item_id as ExamItemId
                , COUNT(case when 集計結果.集計ステータス = 11 then 1 end) as Count11
                , COUNT(case when 集計結果.集計ステータス = 21 then 1 end) as Count21
                , COUNT(case when 集計結果.集計ステータス = 41 then 1 end) as Count41
                , COUNT(case when 集計結果.集計ステータス = 51 then 1 end) as Count51 
            from
                集計結果 
            group by
                exam_item_id
        ) 
        select
            ag.examitemid as ExamItemId
            , items.name as ExamItemName
            , ag.count11 as Count11
            , ag.count21 as Count21
            , ag.count41 as Count41
            , ag.count51 as Count51 
        from
            検査項目集計 ag 
            left join resultcollector.exam_items items 
                on ag.ExamItemId = items.exam_item_id 
            left join resultcollector.exam_item_groups groups 
                on items.exam_item_group_id = groups.exam_item_group_id 
            left join resultcollector.exam_menus menus 
                on groups.exam_menu_id = menus.exam_menu_id
        order by
            menus.order_number
            , groups.order_number
            , items.order_number;";

        var response = await connection.QueryAsync<AggregatedProgressDetail>(sql, new { PlaceScheduleId = placeScheduleId });

        return new AggregatedProgress()
        {
            PlaceScheduleId = placeScheduleId,
            AggregatedProgressDetails = response
        };
    }
}
