using FluentAssertions;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models;

public class RangeErrorTest
{
    [Fact]
    public void エラーレベル正常のメッセージ()
    {
        // Arrange
        var rangeError = new RangeError
        {
            RangeId = Guid.Parse("1c9463bd-ab3e-4926-a745-6cceae37112d"),
            Name = "30以上150未満",
            ExamItemDetailId = 1,
            MinValue = 30,
            MaxValue = 150,
            ErrorLevel = InputErrorLevel.正常
        };

        // Act
        var message = rangeError.Message;

        // Assert
        message.Should().Be("");
    }

    [Fact]
    public void エラーレベル警告のメッセージ()
    {
        // Arrange
        var rangeError = new RangeError
        {
            RangeId = Guid.Parse("1c9463bd-ab3e-4926-a745-6cceae37112d"),
            Name = "30以上150未満",
            ExamItemDetailId = 1,
            MinValue = 30,
            MaxValue = 150,
            ErrorLevel = InputErrorLevel.警告
        };

        // Act
        var message = rangeError.Message;

        // Assert
        message.Should().Be("入力値を確認してください。");
    }

    [Fact]
    public void エラーレベル異常のメッセージ()
    {
        // Arrange
        var rangeError = new RangeError
        {
            RangeId = Guid.Parse("1c9463bd-ab3e-4926-a745-6cceae37112d"),
            Name = "30以上150未満",
            ExamItemDetailId = 1,
            MinValue = 30,
            MaxValue = 150,
            ErrorLevel = InputErrorLevel.異常
        };

        // Act
        var message = rangeError.Message;

        // Assert
        message.Should().Be("入力に誤りがあります。");
    }

    [Fact]
    public void 無効なエラーレベル_例外()
    {
        // Arrange
        var rangeError = new RangeError
        {
            RangeId = Guid.NewGuid(),
            Name = "Test Range",
            ExamItemDetailId = 1,
            MaxValue = 100,
            MinValue = 0,
            ErrorLevel = (InputErrorLevel)999 // 無効なエラーレベル
        };

        // Act
        Action act = () => { var message = rangeError.Message; };

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
                    .WithMessage("未対応のエラーレベル: *");
    }
}
