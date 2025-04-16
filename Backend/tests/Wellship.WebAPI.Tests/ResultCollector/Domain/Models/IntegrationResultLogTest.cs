using FluentAssertions;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models;

public class IntegrationResultLogTest
{
    [Fact]
    public void メール表示用の明細テキストを組み立てる()
    {
        // Arrange
        var log = new IntegrationResultLog()
        {
            Id = Guid.Parse("6f20123f-68c6-4d9b-accc-c505eb704643"),
            Summary = "取り込み処理にてワーニングが6件発生しています。",
            LogLevel = Microsoft.Extensions.Logging.LogLevel.Warning,
            ResultCode = "100002",
            ResultCodeName = "除外レコード有",
            FunctionCode = "AP6102",
            FunctionName = "日次ファイル取り込み処理",
            Details = [
                new IntegrationResultLogDetail(){
                    OrderNumber = 1,
                    FunctionCode = "EC2009",
                    FunctionName = "EC2009の名称",
                    EventSource = "基準値(範囲)_20250324.csv Row:2",
                    ResultDetailCode = "RDC001",
                    ResultDetailMessage = "指定されたコードがマスタに登録されていません。",
                    Properties = [
                        new IntegrationResultLogDetailProperty(){Name = "ExamItemDetailCd", Value = "EIDC001"}
                    ]
                },
                new IntegrationResultLogDetail(){
                    OrderNumber = 2,
                    FunctionCode = "EC2004",
                    FunctionName = "EC2004の名称",
                    EventSource = "予約情報_20250324.csv Row:4",
                    ResultDetailCode = "RDC001",
                    ResultDetailMessage = "指定されたコードがマスタに登録されていません。",
                    Properties = [
                        new IntegrationResultLogDetailProperty(){Name = "TeamCode", Value = "TC001"}
                    ]
                },
                new IntegrationResultLogDetail(){
                    OrderNumber = 3,
                    FunctionCode = "EC2004",
                    FunctionName = "EC2004の名称",
                    EventSource = "予約情報_20250324.csv Row:2",
                    ResultDetailCode = "RDC001",
                    ResultDetailMessage = "指定されたコードがマスタに登録されていません。",
                    Properties = [
                        new IntegrationResultLogDetailProperty(){Name = "PlaceCode", Value = "PC001"},
                        new IntegrationResultLogDetailProperty(){Name = "TeamCode", Value = "TC001"},
                        new IntegrationResultLogDetailProperty(){Name = "ExamDate", Value = "2025-10-23"}
                    ]
                },
                new IntegrationResultLogDetail(){
                    OrderNumber = 4,
                    FunctionCode = "EC2004",
                    FunctionName = "EC2004の名称",
                    EventSource = "予約情報_20250324.csv Row:5",
                    ResultDetailCode = "RDC001",
                    ResultDetailMessage = "指定されたコードがマスタに登録されていません。",
                    Properties = [
                        new IntegrationResultLogDetailProperty(){Name = "ExamineeCd", Value = "EX001"}
                    ]
                },
                new IntegrationResultLogDetail(){
                    OrderNumber = 5,
                    FunctionCode = "EC2002",
                    FunctionName = "EC2002の名称",
                    EventSource = "受付情報_20250324.csv Row:3",
                    ResultDetailCode = "RDC002",
                    ResultDetailMessage = "値が登録されていません。",
                    Properties = [
                        new IntegrationResultLogDetailProperty(){Name = "TicketNumber", Value = "TN001"}
                    ]
                },
                new IntegrationResultLogDetail(){
                    OrderNumber = 6,
                    FunctionCode = "EC2002",
                    FunctionName = "EC2002の名称",
                    EventSource = "受付情報_20250324.csv Row:2",
                    ResultDetailCode = "RDC001",
                    ResultDetailMessage = "指定されたコードがマスタに登録されていません。",
                    Properties = [
                        new IntegrationResultLogDetailProperty(){Name = "ConnectionCode", Value = "CC001"}
                    ]
                }
            ],
            OccurredAt = DateTimeOffset.Parse("2025/05/23T04:20:14"),
        };

        // Act
        var emailDetailsText = log.EmailDetailsText;

        // Assert
        string expected = @"詳細機能ID: EC2009
基準値(範囲)_20250324.csv Row:2 指定されたコードがマスタに登録されていません。 ExamItemDetailCd

詳細機能ID: EC2004
予約情報_20250324.csv Row:4 指定されたコードがマスタに登録されていません。 TeamCode
予約情報_20250324.csv Row:2 指定されたコードがマスタに登録されていません。 PlaceCode/TeamCode/ExamDate
予約情報_20250324.csv Row:5 指定されたコードがマスタに登録されていません。 ExamineeCd

詳細機能ID: EC2002
受付情報_20250324.csv Row:3 値が登録されていません。 TicketNumber
受付情報_20250324.csv Row:2 指定されたコードがマスタに登録されていません。 ConnectionCode";
        emailDetailsText.Should().Be(expected);
    }
}
