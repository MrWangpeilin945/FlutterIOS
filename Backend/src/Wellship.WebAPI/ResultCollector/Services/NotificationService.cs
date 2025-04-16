using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Email;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Services;

/// <summary>
/// 通知サービス
/// 連携処理結果をメール通知するためのサービスです。
/// </summary>
public sealed class NotificationService : INotificationService
{

    private readonly INotificationRepository _notificationRepository;
    private readonly IIntegrationResultRepository _integrationResultRepository;
    private readonly IEmailSender _emailSender;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public NotificationService(INotificationRepository notificationRepository, IIntegrationResultRepository integrationResultRepository, IEmailSender emailSender)
    {
        _notificationRepository = notificationRepository;
        _integrationResultRepository = integrationResultRepository;
        _emailSender = emailSender;
    }

    /// <inheritdoc/>
    public async Task SendNotificationEmailsAsync()
    {
        // 通知ルールを取得する
        var rules = await _notificationRepository.GetEnabledNotificationRulesAsync();
        var groupIds = rules.Select(x => x.NotificationGroupId).Distinct().ToList();
        var logLevels = rules.SelectMany(x => x.LogLevels).Distinct().ToList();

        // 通知先一覧を取得する
        var groups = await _notificationRepository.GetNotificationGroupsAsync(groupIds);

        // 通知テンプレートを取得する
        var templates = await _notificationRepository.GetNotificationTemplatesAsync(groupIds);

        // 送信対象ログ一覧を取得する
        var targetIds = await _notificationRepository.GetTargetLogIdsAsync(logLevels);

        // 連携処理結果ログ一覧を取得する
        var logs = await _integrationResultRepository.GetIntegrationResultLogsAsync(targetIds.ToList());

        // NotificationEmailBuilderでメールを組み立てる
        var emailBuilder = new NotificationEmailBuilder(rules, groups, templates, logs);
        var (emails, sendHistories) = emailBuilder.BuildEmails();

        // EmailSenderでメール送信する
        await _emailSender.SendBulkEmailAsync(emails);

        // 通知送信履歴を記録する
        await _notificationRepository.SaveNotificationSendHistoriesAsync(sendHistories);
    }
}
