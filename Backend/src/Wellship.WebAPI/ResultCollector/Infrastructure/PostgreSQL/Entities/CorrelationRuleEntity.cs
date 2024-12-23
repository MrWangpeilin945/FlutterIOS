namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 検査結果相関ルールエンティティ
/// </summary>
public class CorrelationRuleEntity
{
    /// <summary>
    /// 検査結果相関ルールID
    /// </summary>
    public int CorrelationRuleId { get; set; }

    /// <summary>
    /// 検査結果相関ルール名 
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
    /// 検査項目ID_出力用
    /// </summary>
    public int ExamItemId { get; set; }

    /// <summary>
    /// 出力メッセージ
    /// </summary>
    public required string Message { get; set; }
}
