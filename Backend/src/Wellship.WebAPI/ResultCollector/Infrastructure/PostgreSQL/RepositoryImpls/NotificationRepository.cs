using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// 通知リポジトリ
/// </summary>
public class NotificationRepository : INotificationRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// リポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    /// <param name="timeProvider">timeProvider</param>
    public NotificationRepository(IDbConnectionProvider dbConnectionProvider, TimeProvider timeProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
        _timeProvider = timeProvider;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<NotificationRule>> GetEnabledNotificationRulesAsync()
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            notification_group_id as NotificationGroupId
            , log_level as LogLevel
        from
            resultcollector.notification_rules 
        where
            enabled;";

        var rules = await connection.QueryAsync<NotificationRuleEntity>(sql);

        return rules.GroupBy(x => x.NotificationGroupId)
                    .Select(rule => new NotificationRule()
                    {
                        NotificationGroupId = rule.Key,
                        LogLevels = rule.Select(x => (LogLevel)x.LogLevel).ToList()
                    });
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<NotificationGroup>> GetNotificationGroupsAsync(List<int> notificationGroupIds)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            g.id as GroupId
            , g.name as GroupName
            , g.order_number as GroupOrderNumber
            , r.id as RecipientId
            , r.name as RecipientName
            , r.display_name as RecipientDisplayName
            , r.email_address as RecipientEmailAddress
            , r.order_number as RecipientOrderNumber 
        from
            resultcollector.notification_recipients r 
            inner join resultcollector.notification_groups g 
                on r.notification_group_id = g.id 
        where
            g.id = any (@NotificationGroupIds)
        order by
            g.order_number
            , r.order_number;";

        var results = await connection.QueryAsync<NotificationRecipientEntity>(sql, new { NotificationGroupIds = notificationGroupIds });

        return results.GroupBy(x => x.GroupId)
                      .Select(group => new NotificationGroup()
                      {
                          Id = group.Key,
                          Name = group.First().GroupName,
                          OrderNumber = group.First().GroupOrderNumber,
                          Recipients = group.Select(rec => new NotificationRecipient()
                          {
                              Id = rec.RecipientId,
                              Name = rec.RecipientName,
                              DisplayName = rec.RecipientDisplayName,
                              EmailAddress = rec.RecipientEmailAddress,
                              OrderNumber = rec.RecipientOrderNumber
                          }).ToList()
                      });
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<NotificationTemplate>> GetNotificationTemplatesAsync(List<int> notificationGroupIds)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            t.id
            , t.name
            , t.subject
            , t.body
            , t.sender_address as SenderAddress
            , t.notification_group_id as NotificationGroupId 
        from
            resultcollector.notification_templates t 
        where
            t.notification_group_id = any (@NotificationGroupIds);";

        var results = await connection.QueryAsync<NotificationTemplateEntity>(sql, new { NotificationGroupIds = notificationGroupIds });

        return results.Select(x => new NotificationTemplate()
        {
            Id = x.Id,
            Name = x.Name,
            Subject = x.Subject,
            Body = x.Body,
            SenderAddress = x.SenderAddress,
            NotificationGroupId = x.NotificationGroupId
        });
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Guid>> GetTargetLogIdsAsync(List<LogLevel> logLevels)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();

        var intLogLevels = logLevels.Select(x => (int)x).ToArray();

        // 未送信の連携処理結果ログIDを抽出する
        // 連携処理結果ログに対して通知送信履歴を外部結合して、送信履歴に存在しないログIDを取得する
        const string sql = @"
        select distinct
            log.id 
        from
            resultcollector.integration_result_logs log 
            left join resultcollector.notification_send_histories nsh 
                on log.id = nsh.log_id 
        where
            nsh.log_id is null 
            and log.log_level = any (@LogLevels);";

        var results = await connection.QueryAsync<Guid>(sql, new { LogLevels = intLogLevels });
        return results;
    }

    /// <inheritdoc/>
    public async Task SaveNotificationSendHistoriesAsync(IEnumerable<NotificationSendHistory> sendHistories)
    {
        const string createdBy = "NotificationSender";
        var operationTime = _timeProvider.GetUtcNow();

        var connection = await _dbConnectionProvider.GetOrOpenAsync();

        var mainParams = sendHistories.Select(x => new
        {
            Id = Guid.NewGuid(),
            LogId = x.LogId,
            Subject = x.Subject,
            Body = x.Body,
            NotificationGroupId = x.NotificationGroupId,
            SenderAddress = x.SenderAddress,
            CreatedAt = operationTime,
            CreatedBy = createdBy
        }).ToList();

        var recipientParams = sendHistories.SelectMany(x => x.Recipients.Select(r => new
        {
            // このメソッド1回の呼び出しでは、同じログレコードを複数処理しない前提でログIDをキーにレコードのIDを設定する
            // データベース的には同じログIDの履歴コードを複数作成できる
            Id = mainParams.First(mp => mp.LogId == x.LogId).Id,
            RecipientId = r.RecipientId,
            RecipientAddress = r.RecipientAddress
        })).ToList();

        const string mainSql = @"
        insert 
        into resultcollector.notification_send_histories( 
            id
            , log_id
            , subject
            , body
            , notification_group_id
            , sender_address
            , created_at
            , created_by
        ) 
        values ( 
            @Id
            , @LogId
            , @Subject
            , @Body
            , @NotificationGroupId
            , @SenderAddress
            , @CreatedAt
            , @CreatedBy
        );";

        const string recipientSql = @"
        insert 
        into resultcollector.notification_send_history_recipients( 
            id
            , recipient_id
            , recipient_address
            , created_at
            , created_by
        ) 
        values ( 
            @Id
            , @RecipientId
            , @RecipientAddress
            , @CreatedAt
            , @CreatedBy
        );";

        await connection.ExecuteAsync(mainSql, mainParams);
        await connection.ExecuteAsync(recipientSql, recipientParams);
    }
}
