using FluentAssertions;

using Microsoft.Extensions.Logging;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models
{
    public class AppLogTest
    {
        [Fact]
        public void ログレベルとメッセージが正しく設定されること()
        {
            // Arrange
            var logLevel = LogLevel.Information;
            var message = "ログのメッセージです。";
            var details = new Dictionary<string, object> { { "ErrorCode", "E004" }, { "情報1", 123 } };

            // Act
            var appLog = new AppLog
            {
                LogLevel = logLevel,
                Message = message,
                Details = details
            };

            // Assert
            appLog.LogLevel.Should().Be(logLevel);
            appLog.Message.Should().Be(message);
            appLog.Details.Should().BeEquivalentTo(details);
        }

        [Fact]
        public void 付加情報がnullの場合は空のJSONオブジェクトを返すこと()
        {
            // Arrange
            var appLog = new AppLog
            {
                LogLevel = LogLevel.Information,
                Message = "ログのメッセージです。",
                Details = null
            };

            // Act
            var detailsJson = appLog.DetailsJsonString;

            // Assert
            detailsJson.Should().Be("{}");
        }

        [Fact]
        public void 付加情報が正しくJSON文字列に変換されること()
        {
            // Arrange
            var details = new Dictionary<string, object>
            {
                { "ErrorCode", "E0003" },
                { "情報1", 2 }
            };
            var appLog = new AppLog
            {
                LogLevel = LogLevel.Information,
                Message = "テストメッセージ",
                Details = details
            };

            // Act
            var detailsJson = appLog.DetailsJsonString;

            // Assert
            var expectedJson = @"{
  ""ErrorCode"": ""E0003"",
  ""情報1"": 2
}";
            detailsJson.Should().Be(expectedJson);
        }
    }
}
