namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 通知先グループ
/// </summary>
public sealed class NotificationGroup
{
    /// <summary>
    /// 通知先グループID
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// 名称
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 表示順
    /// </summary>
    public required int OrderNumber { get; init; }

    /// <summary>
    /// 通知先リスト
    /// </summary>
    public required List<NotificationRecipient> Recipients { get; init; }
}
