using Ryobi.Wellship.WebAPI.ResultCollector.Utilities;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 通知メールビルダー
/// </summary>
public sealed class NotificationEmailBuilder
{
    private readonly IEnumerable<NotificationRule> _rules;
    private readonly IEnumerable<NotificationGroup> _groups;
    private readonly IEnumerable<NotificationTemplate> _templates;
    private readonly IEnumerable<IntegrationResultLog> _resultLogs;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="rules">通知ルールリスト</param>
    /// <param name="groups">通知先グループリスト</param>
    /// <param name="templates">テンプレートリスト</param>
    /// <param name="resultLogs">連携処理結果ログリスト</param>
    public NotificationEmailBuilder(IEnumerable<NotificationRule> rules,
                                    IEnumerable<NotificationGroup> groups,
                                    IEnumerable<NotificationTemplate> templates,
                                    IEnumerable<IntegrationResultLog> resultLogs)
    {
        _rules = rules;
        _groups = groups;
        _templates = templates;
        _resultLogs = resultLogs;
    }

    /// <summary>
    /// メールを組み立てます。
    /// 通知先グループとログレベルの組み合わせで作成します。
    /// 送信履歴に保存するために標準的なメールオブジェクトとは別に履歴オブジェクトを返却します。
    /// </summary>
    public (List<Email>, List<NotificationSendHistory>) BuildEmails()
    {
        var emails = new List<Email>();
        var sendHistories = new List<NotificationSendHistory>();

        // 通知先グループ毎
        foreach (var rule in _rules)
        {
            var template = GetTemplateForRule(rule);
            if (template is null)
            {
                continue;
            }

            var group = GetGroupForRule(rule);
            if (group is null)
            {
                continue;
            }

            var recipients = group.Recipients.OrderBy(x => x.OrderNumber).ToList();

            // ログレベル毎
            foreach (var loglevel in rule.LogLevels)
            {
                var targetLogs = _resultLogs.Where(x => x.LogLevel == loglevel);

                // ログレコード毎
                foreach (var targetLog in targetLogs)
                {
                    var email = CreateEmail(template, recipients, targetLog);
                    emails.Add(email);

                    var history = CreateSendHistory(group, recipients, targetLog, email);
                    sendHistories.Add(history);
                }
            }
        }

        return (emails, sendHistories);
    }

    /// <summary>
    /// ルールに対応するテンプレートを取得します。
    /// </summary>
    private NotificationTemplate? GetTemplateForRule(NotificationRule rule)
    {
        return _templates.SingleOrDefault(x => x.NotificationGroupId == rule.NotificationGroupId);
    }

    /// <summary>
    /// ルールに対応する通知先グループを取得します。
    /// </summary>
    private NotificationGroup? GetGroupForRule(NotificationRule rule)
    {
        return _groups.SingleOrDefault(x => x.Id == rule.NotificationGroupId);
    }

    /// <summary>
    /// メールを作成します。
    /// </summary>
    private Email CreateEmail(NotificationTemplate template, List<NotificationRecipient> recipients, IntegrationResultLog targetLog)
    {
        // 通知先が複数件あるとき、読点で結合する
        // 表示用名称が空の場合は表示しない
        var filteredRecipients = recipients.Where(x => !string.IsNullOrEmpty(x.DisplayName)).OrderBy(x => x.OrderNumber).Select(x => x.DisplayName);
        var recipientNameList = string.Join("、", filteredRecipients);

        var placeholders = new Dictionary<string, string>{
            { "RecipientNameList", recipientNameList },
            { "ResultCode", targetLog.ResultCode },
            { "ResultName", targetLog.ResultCodeName },
            { "FunctionCode", targetLog.FunctionCode },
            { "FunctionName", targetLog.FunctionName },
            { "LogLevel", targetLog.LogLevel.ToNLogLevel().ToString().ToUpper() },
            { "Summary", targetLog.Summary },
            { "Details", targetLog.EmailDetailsText},
            { "OccurredAt", targetLog.OccurredAt.ToString("yyyy/MM/dd HH:mm:ss") }
        };

        return new Email
        {
            Subject = template.FillSubjectPlaceholders(placeholders),
            Body = template.FillBodyPlaceholders(placeholders),
            Recipients = recipients.Select(x => x.EmailAddress).ToList(),
            Sender = template.SenderAddress
        };
    }

    /// <summary>
    /// 送信履歴を作成します。
    /// </summary>
    private NotificationSendHistory CreateSendHistory(NotificationGroup group, List<NotificationRecipient> recipients, IntegrationResultLog targetLog, Email email)
    {
        return new NotificationSendHistory
        {
            LogId = targetLog.Id,
            Subject = email.Subject,
            Body = email.Body,
            SenderAddress = email.Sender,
            NotificationGroupId = group.Id,
            Recipients = recipients.Select(x => new NotificationSendHistoryRecipient()
            {
                RecipientId = x.Id,
                RecipientAddress = x.EmailAddress
            })
        };
    }
}
