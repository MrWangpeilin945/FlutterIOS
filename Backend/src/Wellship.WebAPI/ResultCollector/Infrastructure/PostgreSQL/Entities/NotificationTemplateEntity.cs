namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 通知テンプレートエンティティ
/// </summary>
public class NotificationTemplateEntity
{
    /// <summary>
    /// テンプレートID
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// テンプレート名
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 件名
    /// </summary>
    public required string Subject { get; init; }

    /// <summary>
    /// 本文
    /// </summary>
    public required string Body { get; init; }

    /// <summary>
    /// 差出人メールアドレス
    /// </summary>
    public required string SenderAddress { get; init; }

    /// <summary>
    /// 通知グループID
    /// </summary>
    public required int NotificationGroupId { get; init; }
}
