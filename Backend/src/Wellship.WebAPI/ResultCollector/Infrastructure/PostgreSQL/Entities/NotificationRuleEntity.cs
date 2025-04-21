namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 通知ルールエンティティ
/// </summary>
public class NotificationRuleEntity
{
    /// <summary>
    /// 通知グループID
    /// </summary>
    public required int NotificationGroupId { get; set; }

    /// <summary>
    /// ログレベル
    /// </summary>
    public required int LogLevel { get; set; }
}
