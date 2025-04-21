namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 通知先
/// </summary>
public sealed class NotificationRecipient
{
    /// <summary>
    /// 通知先ID
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// 名称
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 表示用名称
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// メールアドレス
    /// </summary>
    public required string EmailAddress { get; init; }

    /// <summary>
    /// 表示順
    /// </summary>
    public required int OrderNumber { get; init; }
}
