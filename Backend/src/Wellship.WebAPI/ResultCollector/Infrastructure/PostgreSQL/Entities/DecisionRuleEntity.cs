namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 検査実施判断ルールエンティティ
/// </summary>
public class DecisionRuleEntity
{
    /// <summary>
    /// 検査実施判断ルールID
    /// </summary>
    public int DecisionRuleId { get; set; }

    /// <summary>
    /// 検査実施判断ルール名 
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// 検査メニューID
    /// </summary>
    public int ExamMenuId { get; set; }

    /// <summary>
    /// 優先度
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// 発火条件種別
    /// </summary>
    public int TriggerType { get; set; }

    /// <summary>
    /// エラーレベル
    /// </summary>
    public int ErrorLevel { get; set; }

    /// <summary>
    /// 出力メッセージ
    /// </summary>
    public required string Message { get; set; }
}
