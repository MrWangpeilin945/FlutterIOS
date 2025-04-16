namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 通知送信履歴
/// </summary>
public sealed class NotificationSendHistory
{
    /// <summary>
    /// ログID
    /// </summary>
    public required Guid LogId { get; init; }

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
    /// 通知先グループID
    /// </summary>
    public required int NotificationGroupId { get; init; }

    /// <summary>
    /// 通知先
    /// </summary>
    public required IEnumerable<NotificationSendHistoryRecipient> Recipients { get; init; }
}
