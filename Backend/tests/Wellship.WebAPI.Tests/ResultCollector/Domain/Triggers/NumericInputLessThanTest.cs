using FluentAssertions;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models.Triggers;

public class NumericInputLessThanTest
{
    [Fact]
    public void 入力1が入力2未満_対象()
    {
        // Arrange
        var inputValues = new List<string> { "100.50", "200.75" };
        var ruleTriggerType = RuleTriggerType.NumericInputLessThan;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, new List<string>(), InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeTrue();
        errorLevel.Should().Be(InputErrorLevel.異常);
    }

    [Fact]
    public void 入力1が入力2以上_対象外()
    {
        // Arrange
        var inputValues = new List<string> { "200.75", "100.50" };
        var ruleTriggerType = RuleTriggerType.NumericInputLessThan;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, new List<string>(), InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeFalse();
        errorLevel.Should().Be(InputErrorLevel.正常);
    }

    [Fact]
    public void 入力1が入力2と等しい_対象外()
    {
        // Arrange
        var inputValues = new List<string> { "100.50", "100.50" };
        var ruleTriggerType = RuleTriggerType.NumericInputLessThan;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, new List<string>(), InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeFalse();
        errorLevel.Should().Be(InputErrorLevel.正常);
    }

    [Fact]
    public void 入力値が不足している_対象外()
    {
        // Arrange
        var inputValues = new List<string> { "100.50" };
        var ruleTriggerType = RuleTriggerType.NumericInputLessThan;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, new List<string>(), InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeFalse();
        errorLevel.Should().Be(InputErrorLevel.正常);
    }

    [Fact]
    public void 入力値が数値でない_対象外()
    {
        // Arrange
        var inputValues = new List<string> { "abc", "100.50" };
        var ruleTriggerType = RuleTriggerType.NumericInputLessThan;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, new List<string>(), InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeFalse();
        errorLevel.Should().Be(InputErrorLevel.正常);
    }
}
