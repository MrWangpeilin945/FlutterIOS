using FluentAssertions;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models;

public class NotificationTemplateTests
{

    private const string DetailText = @"詳細機能ID: EC2009
基準値(範囲)_20250324.csv Row:2 指定されたコードがマスタに登録されていません。 ExamItemDetailCd

詳細機能ID: EC2004
予約情報_20250324.csv Row:4 指定されたコードがマスタに登録されていません。 TeamCode
予約情報_20250324.csv Row:2 指定されたコードがマスタに登録されていません。 PlaceCode/TeamCode/ExamDate
予約情報_20250324.csv Row:5 指定されたコードがマスタに登録されていません。 ExamineeCd

詳細機能ID: EC2002
受付情報_20250324.csv Row:3 値が登録されていません。 TicketNumber
受付情報_20250324.csv Row:2 指定されたコードがマスタに登録されていません。 ConnectionCode";
    private readonly Dictionary<string, string> _placeholders = new Dictionary<string, string>
    {
        { "RecipientNameList", "情報システム課 ご担当者様、青空 太郎 様"},
        { "ResultCode", "100" },
        { "ResultName", "Success" },
        { "FunctionCode", "Func001" },
        { "FunctionName", "機能A" },
        { "LogLevel", "INFO" },
        { "Summary", "これは概要です。" },
        { "OccurredAt", "2025/03/28 15:03:55" },
        {"Details" , DetailText}
    };

    [Fact]
    public void 件名のプレースホルダが値で置き換えられる()
    {
        // Arrange
        var template = new NotificationTemplate
        {
            Id = 1,
            Name = "Test Template",
            Subject = "[WELLSHIP連携]連携処理通知 {{LogLevel}}",
            Body = "",
            SenderAddress = "no-reply@example.com",
            NotificationGroupId = 1
        };

        // Act
        var result = template.FillSubjectPlaceholders(_placeholders);

        // Assert
        result.Should().Be("[WELLSHIP連携]連携処理通知 INFO");
    }

    [Fact]
    public void 本文のプレースホルダが値で置き換えられる()
    {
        // Arrange
        string templateBody = @"宛先）{{RecipientNameList}}

---
処理結果コード：{{ResultCode}}
処理結果名：{{ResultName}}
機能コード：{{FunctionCode}}
機能名：{{FunctionName}}
ログレベル：{{LogLevel}}
概要：{{Summary}}
---
発生時刻：{{OccurredAt}}
---
詳細：
{{Details}}

以上";

        var template = new NotificationTemplate
        {
            Id = 1,
            Name = "Test Template",
            Subject = "",
            Body = templateBody,
            SenderAddress = "no-reply@example.com",
            NotificationGroupId = 1
        };

        string expected = @"宛先）情報システム課 ご担当者様、青空 太郎 様

---
処理結果コード：100
処理結果名：Success
機能コード：Func001
機能名：機能A
ログレベル：INFO
概要：これは概要です。
---
発生時刻：2025/03/28 15:03:55
---
詳細：
詳細機能ID: EC2009
基準値(範囲)_20250324.csv Row:2 指定されたコードがマスタに登録されていません。 ExamItemDetailCd

詳細機能ID: EC2004
予約情報_20250324.csv Row:4 指定されたコードがマスタに登録されていません。 TeamCode
予約情報_20250324.csv Row:2 指定されたコードがマスタに登録されていません。 PlaceCode/TeamCode/ExamDate
予約情報_20250324.csv Row:5 指定されたコードがマスタに登録されていません。 ExamineeCd

詳細機能ID: EC2002
受付情報_20250324.csv Row:3 値が登録されていません。 TicketNumber
受付情報_20250324.csv Row:2 指定されたコードがマスタに登録されていません。 ConnectionCode

以上";

        // Act
        var result = template.FillBodyPlaceholders(_placeholders);

        // Assert
        result.Should().Be(expected);
    }
}
