using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// 通知リポジトリのインターフェース
/// </summary>
public interface INotificationRepository
{
    /// <summary>
    /// 有効な通知ルールを取得します。
    /// </summary>
    /// <returns>通知ルールのリスト</returns>
    public Task<IEnumerable<NotificationRule>> GetEnabledNotificationRulesAsync();

    /// <summary>
    /// 通知先グループを取得します。
    /// </summary>
    /// <param name="notificationGroupIds">通知先グループIDリスト</param>
    /// <returns>通知先グループのリスト</returns>
    public Task<IEnumerable<NotificationGroup>> GetNotificationGroupsAsync(List<int> notificationGroupIds);

    /// <summary>
    /// 通知テンプレートを取得します。
    /// </summary>
    /// <param name="notificationGroupIds">通知先グループIDリスト</param>
    /// <returns>通知テンプレートのリスト</returns>
    public Task<IEnumerable<NotificationTemplate>> GetNotificationTemplatesAsync(List<int> notificationGroupIds);

    /// <summary>
    /// 指定されたログレベルの送信対象ログIDを取得します。
    /// </summary>
    /// <param name="logLevels">ログレベル</param>
    /// <returns>送信対象ログIDのリスト</returns>
    public Task<IEnumerable<Guid>> GetTargetLogIdsAsync(List<LogLevel> logLevels);

    /// <summary>
    /// 通知送信履歴を保存します。
    /// </summary>
    /// <param name="sendHistories">通知送信履歴リスト</param>
    public Task SaveNotificationSendHistoriesAsync(IEnumerable<NotificationSendHistory> sendHistories);
}
