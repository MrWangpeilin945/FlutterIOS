namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 通知ルール
/// </summary>
public sealed class NotificationRule
{
    /// <summary>
    /// 通知先グループID
    /// </summary>
    public required int NotificationGroupId { get; init; }

    /// <summary>
    /// 送信対象のログレベル一覧
    /// </summary>
    public required List<LogLevel> LogLevels { get; init; }
}
