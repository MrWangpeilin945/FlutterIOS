using FluentAssertions;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models.Triggers;

public class NumericConditionNotEqualTest
{
    [Fact]
    public void 入力1と判定1が異なる_対象()
    {
        // Arrange
        var inputValues = new List<string> { "100.50" };
        var conditionValues = new List<string> { "200.75" };
        var ruleTriggerType = RuleTriggerType.NumericConditionNotEqual;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeTrue();
        errorLevel.Should().Be(InputErrorLevel.異常);
    }

    [Fact]
    public void 入力1と判定1が等しい_対象外()
    {
        // Arrange
        var inputValues = new List<string> { "100.50" };
        var conditionValues = new List<string> { "100.50" };
        var ruleTriggerType = RuleTriggerType.NumericConditionNotEqual;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

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
        var conditionValues = new List<string>();
        var ruleTriggerType = RuleTriggerType.NumericConditionNotEqual;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeFalse();
        errorLevel.Should().Be(InputErrorLevel.正常);
    }

    [Fact]
    public void 判定値が不足している_対象外()
    {
        // Arrange
        var inputValues = new List<string>();
        var conditionValues = new List<string> { "100.50" };
        var ruleTriggerType = RuleTriggerType.NumericConditionNotEqual;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeFalse();
        errorLevel.Should().Be(InputErrorLevel.正常);
    }

    [Fact]
    public void 入力1が数値でない_対象外()
    {
        // Arrange
        var inputValues = new List<string> { "abc" };
        var conditionValues = new List<string> { "100.50" };
        var ruleTriggerType = RuleTriggerType.NumericConditionNotEqual;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeFalse();
        errorLevel.Should().Be(InputErrorLevel.正常);
    }

    [Fact]
    public void 判定1が数値でない_対象外()
    {
        // Arrange
        var inputValues = new List<string> { "100.50" };
        var conditionValues = new List<string> { "abc" };
        var ruleTriggerType = RuleTriggerType.NumericConditionNotEqual;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeFalse();
        errorLevel.Should().Be(InputErrorLevel.正常);
    }
}