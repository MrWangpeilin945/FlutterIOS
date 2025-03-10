using FluentAssertions;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models.Triggers;

public class StringNotInTest
{
    [Fact]
    public void 入力値が判定値リストに含まれない_対象()
    {
        // Arrange
        var inputValues = new List<string> { "ZZZ" };
        var conditionValues = new List<string> { "AAA", "BBB", "CCC" };
        var ruleTriggerType = RuleTriggerType.StringNotIn;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeTrue();
        errorLevel.Should().Be(InputErrorLevel.異常);
    }

    [Fact]
    public void 入力値が判定値リストに含まれない_空文字_対象()
    {
        // Arrange
        var inputValues = new List<string> { "" };
        var conditionValues = new List<string> { "AAA", "BBB", "CCC" };
        var ruleTriggerType = RuleTriggerType.StringNotIn;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeTrue();
        errorLevel.Should().Be(InputErrorLevel.異常);
    }

    [Fact]
    public void 入力値が判定値リストに含まれる_対象外()
    {
        // Arrange
        var inputValues = new List<string> { "BBB" };
        var conditionValues = new List<string> { "AAA", "BBB", "CCC" };
        var ruleTriggerType = RuleTriggerType.StringNotIn;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeFalse();
        errorLevel.Should().Be(InputErrorLevel.正常);
    }

    [Fact]
    public void 入力値が空_対象外()
    {
        // Arrange
        var inputValues = new List<string>();
        var conditionValues = new List<string> { "test", "example" };
        var ruleTriggerType = RuleTriggerType.StringNotIn;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeFalse();
        errorLevel.Should().Be(InputErrorLevel.正常);
    }

    [Fact]
    public void 判定値リストが空_対象外()
    {
        // Arrange
        var inputValues = new List<string> { "test" };
        var conditionValues = new List<string>();
        var ruleTriggerType = RuleTriggerType.StringNotIn;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeFalse();
        errorLevel.Should().Be(InputErrorLevel.正常);
    }
}
