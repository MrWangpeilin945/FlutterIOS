namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 通知送信履歴の宛先
/// </summary>
public sealed class NotificationSendHistoryRecipient
{
    /// <summary>
    /// 通知先ID
    /// </summary>
    public required int RecipientId { get; init; }

    /// <summary>
    /// 通知先メールアドレス
    /// </summary>
    public required string RecipientAddress { get; init; }
}
