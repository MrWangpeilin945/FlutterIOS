namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 通知先エンティティ
/// </summary>
public class NotificationRecipientEntity
{
    /// <summary>
    /// 通知先グループID
    /// </summary>
    public required int GroupId { get; set; }

    /// <summary>
    /// 通知先グループ名
    /// </summary>
    public required string GroupName { get; set; }

    /// <summary>
    /// 通知先グループの表示順
    /// </summary>
    public int GroupOrderNumber { get; set; }

    /// <summary>
    /// 通知先ID
    /// </summary>
    public int RecipientId { get; set; }

    /// <summary>
    /// 通知先名
    /// </summary>
    public required string RecipientName { get; set; }

    /// <summary>
    /// 通知先の表示名
    /// </summary>
    public required string RecipientDisplayName { get; set; }

    /// <summary>
    /// 通知先のメールアドレス
    /// </summary>
    public required string RecipientEmailAddress { get; set; }

    /// <summary>
    /// 通知先の表示順
    /// </summary>
    public required int RecipientOrderNumber { get; set; }
}
