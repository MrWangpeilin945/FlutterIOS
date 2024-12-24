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

    /// <summary>
    /// 検査結果相関ルールを取得します。
    /// </summary>
    /// <param name="examMenuId">検査メニューID</param>
    public async Task<IEnumerable<CorrelationRule>> GetCorrelationRulesAsync(int examMenuId)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();

        // 検査結果相関ルール_メインテーブル
        const string mainSql = @"
        select
            r.correlation_rule_id as CorrelationRuleId
            , r.name as Name
            , r.exam_menu_id as ExamMenuId
            , r.priority as Priority
            , r.trigger_type as TriggerType
            , r.error_level as ErrorLevel
            , r.exam_item_id as ExamItemId
            , r.message as Message 
        from
            resultcollector.correlation_rules r
        where
            r.exam_menu_id = @ExamMenuId;";

        var correlationRules = await connection.QueryAsync<CorrelationRuleEntity>(mainSql, new { ExamMenuId = examMenuId });

        // 検査結果相関ルール_判定値テーブル
        const string evaluationSql = @"
        select
            r.correlation_rule_id as CorrelationRuleId
            , e.variable_number as VariableNumber
            , e.evaluation_value as EvaluationValue 
        from
            resultcollector.correlation_rules r 
            left join resultcollector.correlation_rule_evaluations e 
                on r.correlation_rule_id = e.correlation_rule_id
        where
            r.exam_menu_id = @ExamMenuId;";

        var evaluations = await connection.QueryAsync<CorrelationRuleEvaluationEntity>(evaluationSql, new { ExamMenuId = examMenuId });

        // 検査結果相関ルール_検査項目明細テーブル
        const string examItemDetailSql = @"
        select
            r.correlation_rule_id as CorrelationRuleId
            , d.variable_number as VariableNumber
            , d.source_type as SourceType
            , d.exam_item_detail_id as ExamItemDetailId 
        from
            resultcollector.correlation_rules r 
            left join resultcollector.correlation_rule_exam_item_details d 
                on r.correlation_rule_id = d.correlation_rule_id
        where
            r.exam_menu_id = @ExamMenuId;";

        var examItemDetails = await connection.QueryAsync<CorrelationRuleExamItemDetailEntity>(examItemDetailSql, new { ExamMenuId = examMenuId });

        return correlationRules.Select(x => new CorrelationRule()
        {
            CorrelationRuleId = x.CorrelationRuleId,
            Name = x.Name,
            ExamMenuId = x.ExamMenuId,
            Priority = x.Priority,
            TriggerType = (RuleTriggerType)x.TriggerType,
            ErrorLevel = (InputErrorLevel)x.ErrorLevel,
            ExamItemId = x.ExamItemId,
            Message = x.Message,
            Evaluations = evaluations.Where(ev => ev.CorrelationRuleId == x.CorrelationRuleId)
                                     .Select(ev => new CorrelationRuleEvaluation()
                                     {
                                         VariableNumber = ev.VariableNumber,
                                         EvaluationValue = ev.EvaluationValue
                                     }),
            ExamItemDetails = examItemDetails.Where(ei => ei.CorrelationRuleId == x.CorrelationRuleId)
                                             .Select(ei => new CorrelationRuleExamItemDetail()
                                             {
                                                 VariableNumber = ei.VariableNumber,
                                                 SourceType = (SourceType)ei.SourceType,
                                                 ExamItemDetailId = ei.ExamItemDetailId
                                             })
        });
    }

    /// <summary>
    /// 検査項目明細とその子要素を取得します。
    /// </summary>
    /// <param name="examItemDetailIds">検査項目明細ID</param>
    public async Task<IEnumerable<ExamItemDetailChild>> GetExamItemDetailChildrenAsync(IEnumerable<int> examItemDetailIds)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select 
            d.exam_item_detail_id as ExamItemDetailId,
            d.name as Name,
            d.position_number as PositionNumber,
            d.equipment_label as EquipmentLabel,
            ex.unit as Unit,
            d.type as Type,
            d.keyboard_type as KeyboardType,
            d.integer_length as IntegerLength,
            d.decimal_length as DecimalLength,
            kb.option_id as KeyboardId,
            kb.value as KeyboardValue,
            op.option_id as OptionId,
            op.code as OptionCode,
            op.name as OptionName,
            op.order_number as OrderNumber
        from
            resultcollector.exam_item_details d 
            left join resultcollector.exam_items ex
                on d.exam_item_id = ex.exam_item_id
            left join resultcollector.keyboard_options kb
                on d.exam_item_detail_id = kb.exam_item_detail_id
            left join resultcollector.exam_item_detail_options op
                on d.exam_item_detail_id = op.exam_item_detail_id
            where
                d.exam_item_detail_id = any(@ExamItemDetailIds);";        
        var examItemDetails = await connection.QueryAsync<ExamItemDetailChildrenEntity>(sql, new { ExamItemDetailIds = examItemDetailIds });

        var examItemDetailChildren = examItemDetails
                                    .GroupBy(d => d.ExamItemDetailId)
                                    .Select(d => new ExamItemDetailChild
                                    {
                                        ExamItemDetailId = d.First().ExamItemDetailId,
                                        Name = d.First().Name,
                                        PositionNumber = d.First().PositionNumber,
                                        EquipmentLabel = d.First().EquipmentLabel,
                                        Unit = d.First().Unit,
                                        Type = d.First().Type,
                                        KeyboardType = d.First().KeyboardType,
                                        Keyboards = d.Where(kb => kb.ExamItemDetailId == d.First().ExamItemDetailId)
                                                     .GroupBy(kb => kb.KeyboardId) 
                                                     .OrderBy(kb => kb.First().KeyboardId)
                                                     .Select(kb => new Keyboard{
                                                        OptionId = kb.First().KeyboardId,
                                                        ExamItemDetailId = kb.First().ExamItemDetailId,
                                                        Value = kb.First().KeyboardValue
                                                    }),
                                        IntegerLength = d.First().IntegerLength,
                                        DecimalLength = d.First().DecimalLength,
                                        DetailOptions = d.Where(op => op.ExamItemDetailId == d.First().ExamItemDetailId)
                                                         .GroupBy(op => op.OptionId) 
                                                         .OrderBy(op => op.First().OptionId)
                                                         .Select(op => new ExamItemDetailOption{
                                                            Code = op.First().OptionCode,
                                                            ExamItemDetailId = op.First().ExamItemDetailId,
                                                            Name = op.First().OptionName,
                                                            OrderNumber = op.First().OrderNumber
                                                        })
                                    });
        return examItemDetailChildren;
    }
}
