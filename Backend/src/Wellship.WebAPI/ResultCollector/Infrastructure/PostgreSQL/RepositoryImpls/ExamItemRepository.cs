
using System.Runtime.Intrinsics.Arm;
using Dapper;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// 検査項目リポジトリ
/// /// </summary>
public class ExamItemRepository : IExamItemRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    /// <summary>
    /// リポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    public ExamItemRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    /// <summary>
    /// 検査メニューに関連した検査項目情報を取得します。
    /// </summary>
    public async Task<IEnumerable<ExamItemGroup>> GetExamItemGroupsAsync(int examMenuId)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select 
            g.exam_item_group_id as ExamItemGroupId
            , g.type as GroupType 
            , i.position_number as ExamItemPositionNumber 
            , i.exam_item_id as ExamItemId
            , i.name as ExamItemName
            , d.position_number as ExamItemDetailPositionNumber 
            , d.exam_item_detail_id as ExamItemDetailId 
            , d.equipment_label as EquipmentLabel
            , d.name as ExamItemDetailName
            , i.unit as Unit
            , d.type as ExamItemDetailType
            , integer_length as IntegerLength
            , decimal_length as DecimalLength 
            , d.keyboard_type as KeyboardType
        from resultcollector.exam_item_groups g
        left join resultcollector.exam_items  i
            on g.exam_item_group_id = i.exam_item_group_id
        left join resultcollector.exam_item_details as d
            on d.exam_item_id = i.exam_item_id
        where g.exam_menu_id=@ExamMenuId
        order by
            g.exam_item_group_id
            , i.position_number
            , i.exam_item_id
            , d.order_number;";

        var results = await connection.QueryAsync<ExamItemGroupEntity>(sql, new { ExamMenuId = examMenuId });
        var examItemGroups = 
            results
            .GroupBy(g => g.ExamItemGroupId)
            .Select(eg => new ExamItemGroup
            {
                ExamItemGroupId = eg.First().ExamItemGroupId,
                Type = eg.First().GroupType,
                //検査項目
                ExamItems = 
                        eg.GroupBy(e => e.ExamItemId)
                            .Select(ei => new ExamItem
                            {
                                PositionNumber = ei.First().ExamItemPositionNumber,
                                ExamItemId = ei.First().ExamItemId,
                                Name = ei.First().ExamItemName,
                                // 検査項目明細
                                ExamItemDetails 
                                    = ei.Select(ed => new ExamItemDetail
                                    {
                                        PositionNumber = ed.ExamItemDetailPositionNumber,
                                        ExamItemDetailId = ed.ExamItemDetailId,
                                        EquipmentLabel = ed.EquipmentLabel,
                                        Name = ed.ExamItemDetailName,
                                        Unit = ed.Unit,
                                        Type = ed.ExamItemDetailType,
                                        IntegerLength = ed.IntegerLength,
                                        DecimalLength = ed.DecimalLength,
                                        KeyboardType = ed.KeyboardType
                                    })
                            })    
            });
        return examItemGroups;
    }

    /// <summary>
    /// キーボード入力値リストを取得します。
    /// </summary>
    /// <param name="examItemDetailIds">検査項目明細ID</param>
    /// <returns></returns>
    public async Task<IEnumerable<Keyboard>> GetKeyboardOptionssAsync(int[] examItemDetailIds)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select 
            option_id as OptionId
            , exam_item_detail_id as ExamItemDetailId
            , value as Value
        from resultcollector.keyboard_options
        where exam_item_detail_id=any(@ExamItemDetailIds)
        order by
            exam_item_detail_id
            , option_id;";
        return await connection.QueryAsync<Keyboard>(sql, new { ExamItemDetailIds = examItemDetailIds });
    }

    /// <summary>
    /// 検査項目明細選択肢を取得します。
    /// </summary>
    /// <param name="examItemDetailIds">検査項目明細ID</param>
    public async Task<IEnumerable<ExamItemDetailOption>> GetExamItemDetailOptionsAsync(int[] examItemDetailIds)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select 
            code as Code
            , exam_item_detail_id as ExamItemDetailId
            , name as Name
            , order_number as OrderNumber
        from resultcollector.exam_item_detail_options
        where exam_item_detail_id=any(@ExamItemDetailIds)
        order by
            order_number;";
        return await connection.QueryAsync<ExamItemDetailOption>(sql, new { ExamItemDetailIds = examItemDetailIds });
    }

    /// <summary>
    /// 検査正常値範囲を取得します。
    /// </summary>
    /// <param name="thresholdIds">基準値パターンID</param>
    /// <param name="examItemDetailIds">検査項目明細ID</param>
    /// <param name="age">受診者の健診時の年齢</param>
    /// <param name="sex">受診者の性別</param>
    public async Task<IEnumerable<ExamNormalValueRange>> GetExamNormalValueRangesAsync(Guid[] thresholdIds, int[] examItemDetailIds, Age age, Sex sex)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select 
            r.name as Name
            , r.threshold_id as ThresholdId
            , r.exam_item_detail_id as ExamItemDetailId
            , r.min_age as MinAge
            , r.max_age as MaxAge
            , r.target_sex as TargetSex
            , r.max_value as MaxValue
            , r.min_value as MinValue
            , r.error_level as ErrorLevel
            , ct.priority as Priority
        from resultcollector.exam_normal_value_range r
        left join resultcollector.consult_thresholds ct 
            on r.threshold_id = ct.threshold_id
        where r.threshold_id = any(@ThresholdIds)
        and r.exam_item_detail_id = any(@ExamItemDetailIds);";
        var normalValueRanges = await connection.QueryAsync<ExamNormalValueRange>(sql, 
                                        new { ThresholdIds = thresholdIds, ExamItemDetailIds = examItemDetailIds });
        return normalValueRanges.Where(x => x.IsTargetAge(age) && x.IsTargetSex(sex));
    }
}
