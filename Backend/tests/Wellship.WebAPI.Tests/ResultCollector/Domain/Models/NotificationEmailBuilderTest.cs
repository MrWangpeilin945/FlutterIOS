using FluentAssertions;

using Microsoft.Extensions.Logging;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models;

public class NotificationEmailBuilderTest
{
    [Fact]
    public void 複数のログレベルに対応するメールを組み立てる()
    {
        // Arrange
        var rules = new List<NotificationRule>
            {
                new() { NotificationGroupId = 1, LogLevels = [LogLevel.Information, LogLevel.Warning, LogLevel.Error] },
                new() { NotificationGroupId = 2, LogLevels = [LogLevel.Error] }
            };

        var groups = new List<NotificationGroup>
        {
            new() {
                Id = 1,
                Name = "すべて送信グループ",
                OrderNumber = 1,
                Recipients = [
                    new NotificationRecipient { Id = 11, Name = "通知先1-1", DisplayName = "通知先1-1_表示名", EmailAddress = "target11@example.com", OrderNumber = 1 },
                    new NotificationRecipient { Id = 12, Name = "通知先1-2", DisplayName = "通知先1-2_表示名", EmailAddress = "target12@example.com", OrderNumber = 2 },
                    new NotificationRecipient { Id = 13, Name = "通知先1-3", DisplayName = "通知先1-3_表示名", EmailAddress = "target13@example.com", OrderNumber = 3 }
                ]
            },
            new() {
                Id = 2,
                Name = "エラーのみ送信グループ",
                OrderNumber = 2,
                Recipients = [
                    new NotificationRecipient { Id = 21, Name = "通知先2-1", DisplayName = "通知先2-1_表示名", EmailAddress = "target21@example.com", OrderNumber = 1 },
                ]
            }
        };

        var templates = new List<NotificationTemplate>
        {
            new NotificationTemplate
            {
                Id = 1,
                Name = "すべて送信テンプレート",
                NotificationGroupId = 1,
                SenderAddress = "no-reply@example.com",
                Subject = "[WELLSHIP連携G1] [{{LogLevel}}] {{Summary}}",
                Body = "本文（組み立てのテストは割愛）"
            },
            new NotificationTemplate
            {
                Id = 2,
                Name = "エラーのみテンプレート",
                NotificationGroupId = 2,
                SenderAddress = "no-reply@example.com",
                Subject = "[WELLSHIP連携G2] [{{LogLevel}}]",
                Body = "本文（組み立てのテストは割愛）"
            }
        };

        var resultLogs = new List<IntegrationResultLog>
        {
            new IntegrationResultLog
            {
                Id = Guid.Parse("cefe228b-888e-46f1-b742-16ef3c800056"),
                LogLevel = LogLevel.Information,
                ResultCode = "100000",
                ResultCodeName = "正常終了",
                FunctionCode = "AP6201",
                FunctionName = "受診者を登録する(WebAPI)",
                Summary = "受診者登録成功",
                OccurredAt = DateTimeOffset.Parse("2025/04/17 14:40:00"),
                Details = []
            },
            new IntegrationResultLog
            {
                Id = Guid.Parse("d35b8e4a-d471-4945-813a-b620d783900b"),
                LogLevel = LogLevel.Warning,
                ResultCode = "100002",
                ResultCodeName = "取り込み除外レコード有",
                FunctionCode = "AP6202",
                FunctionName = "受付を更新する(WebAPI)",
                Summary = "受付更新警告",
                OccurredAt = DateTimeOffset.Parse("2025/04/18 06:00:10"),
                Details = []
            },
            new IntegrationResultLog
            {
                Id = Guid.Parse("7473f404-3644-4937-96e5-65a1be997855"),
                LogLevel = LogLevel.Error,
                ResultCode = "199999",
                ResultCodeName = "異常終了",
                FunctionCode = "AP6202",
                FunctionName = "受付を更新する(WebAPI)",
                Summary = "受付更新エラー",
                OccurredAt = DateTimeOffset.Parse("2025/04/18 06:30:32"),
                Details = []
            }
        };

        var builder = new NotificationEmailBuilder(rules, groups, templates, resultLogs);

        // Act
        var (emails, sendHistories) = builder.BuildEmails();

        // Assert

        emails.Should().HaveCount(4);
        sendHistories.Should().HaveCount(4);

        emails[0].Recipients.Should().BeEquivalentTo(["target11@example.com", "target12@example.com", "target13@example.com"]);
        emails[1].Recipients.Should().BeEquivalentTo(["target11@example.com", "target12@example.com", "target13@example.com"]);
        emails[2].Recipients.Should().BeEquivalentTo(["target11@example.com", "target12@example.com", "target13@example.com"]);
        emails[3].Recipients.Should().BeEquivalentTo(["target21@example.com"]);

        emails[0].Subject.Should().Be("[WELLSHIP連携G1] [INFO] 受診者登録成功");
        emails[1].Subject.Should().Be("[WELLSHIP連携G1] [WARN] 受付更新警告");
        emails[2].Subject.Should().Be("[WELLSHIP連携G1] [ERROR] 受付更新エラー");
        emails[3].Subject.Should().Be("[WELLSHIP連携G2] [ERROR]");

        sendHistories[0].Recipients.Select(x => x.RecipientId).Should().BeEquivalentTo([11, 12, 13]);
        sendHistories[1].Recipients.Select(x => x.RecipientId).Should().BeEquivalentTo([11, 12, 13]);
        sendHistories[2].Recipients.Select(x => x.RecipientId).Should().BeEquivalentTo([11, 12, 13]);
        sendHistories[3].Recipients.Select(x => x.RecipientId).Should().BeEquivalentTo([21]);

        sendHistories[0].Subject.Should().Be("[WELLSHIP連携G1] [INFO] 受診者登録成功");
        sendHistories[1].Subject.Should().Be("[WELLSHIP連携G1] [WARN] 受付更新警告");
        sendHistories[2].Subject.Should().Be("[WELLSHIP連携G1] [ERROR] 受付更新エラー");
        sendHistories[3].Subject.Should().Be("[WELLSHIP連携G2] [ERROR]");
    }

    [Fact]
    public void テンプレートが存在しない場合メールを組み立てない()
    {
        // Arrange
        var rules = new List<NotificationRule>
        {
            new() { NotificationGroupId = 1, LogLevels = [LogLevel.Error] }
        };

        var groups = new List<NotificationGroup>
        {
            new() {
                Id = 1,
                Name = "通知先グループ1",
                OrderNumber = 1,
                Recipients = [
                    new NotificationRecipient { Id = 11, Name = "通知先1-1", DisplayName = "通知先1-1_表示名", EmailAddress = "target11@example.com", OrderNumber = 1 },
                    new NotificationRecipient { Id = 12, Name = "通知先1-2", DisplayName = "通知先1-2_表示名", EmailAddress = "target12@example.com", OrderNumber = 2 },
                    new NotificationRecipient { Id = 13, Name = "通知先1-3", DisplayName = "通知先1-3_表示名", EmailAddress = "target13@example.com", OrderNumber = 3 }
                ]
            }
        };

        // 空のテンプレートリスト
        var templates = new List<NotificationTemplate>();

        var resultLogs = new List<IntegrationResultLog>
        {
            new() {
                Id = Guid.Parse("d5735bac-b1f9-4e24-880c-3127cdf9b076"),
                LogLevel = LogLevel.Error,
                ResultCode = "100000",
                ResultCodeName = "正常終了",
                FunctionCode = "AP6201", FunctionName = "受診者を登録する(WebAPI)",
                Summary = "受診者登録成功",
                OccurredAt = DateTimeOffset.Parse("2025/04/17 14:40:00"),
                Details = []
            }
        };

        var builder = new NotificationEmailBuilder(rules, groups, templates, resultLogs);

        // Act
        var (emails, sendHistories) = builder.BuildEmails();

        // Assert
        emails.Should().BeEmpty();
        sendHistories.Should().BeEmpty();
    }

    [Fact]
    public void グループが存在しない場合メールを組み立てない()
    {
        // Arrange
        var rules = new List<NotificationRule>
        {
            new() { NotificationGroupId = 1, LogLevels = [LogLevel.Error] }
        };

        // 空のグループリスト
        var groups = new List<NotificationGroup>();

        var templates = new List<NotificationTemplate>
        {
            new NotificationTemplate
            {
                Id = 1,
                Name = "通知先グループ1用テンプレート",
                NotificationGroupId = 1,
                SenderAddress = "no-reply@example.com",
                Subject = "[WELLSHIP連携G1] [{{LogLevel}}] {{Summary}}",
                Body = "本文（組み立てのテストは割愛）" }
        };

        var resultLogs = new List<IntegrationResultLog>
        {
            new() {
                Id = Guid.Parse("d5735bac-b1f9-4e24-880c-3127cdf9b076"),
                LogLevel = LogLevel.Error,
                ResultCode = "100000",
                ResultCodeName = "正常終了",
                FunctionCode = "AP6201", FunctionName = "受診者を登録する(WebAPI)",
                Summary = "受診者登録成功",
                OccurredAt = DateTimeOffset.Parse("2025/04/17 14:40:00"),
                Details = []
            }
        };

        var builder = new NotificationEmailBuilder(rules, groups, templates, resultLogs);

        // Act
        var (emails, sendHistories) = builder.BuildEmails();

        // Assert
        emails.Should().BeEmpty();
        sendHistories.Should().BeEmpty();
    }
}
