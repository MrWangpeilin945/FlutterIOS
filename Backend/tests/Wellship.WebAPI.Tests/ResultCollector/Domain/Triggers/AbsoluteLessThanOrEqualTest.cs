using FluentAssertions;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models.Triggers;

public class AbsoluteLessThanOrEqualTest
{
    [Fact]
    public void 入力値の差の絶対値が判定値と等しい_対象()
    {
        // Arrange
        var inputValues = new List<string> { "120", "100" };
        var conditionValues = new List<string> { "20" };
        var ruleTriggerType = RuleTriggerType.AbsoluteLessThanOrEqual;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeTrue();
        errorLevel.Should().Be(InputErrorLevel.異常);
    }

    [Fact]
    public void 入力値の差の絶対値が判定値未満_対象()
    {
        // Arrange
        var inputValues = new List<string> { "119.9", "100" };
        var conditionValues = new List<string> { "20" };
        var ruleTriggerType = RuleTriggerType.AbsoluteLessThanOrEqual;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeTrue();
        errorLevel.Should().Be(InputErrorLevel.異常);
    }

    [Fact]
    public void 入力値の差の絶対値が判定値より大きい_対象外()
    {
        // Arrange
        var inputValues = new List<string> { "120.1", "100" };
        var conditionValues = new List<string> { "20" };
        var ruleTriggerType = RuleTriggerType.AbsoluteLessThanOrEqual;
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
        var inputValues = new List<string> { "100" };
        var conditionValues = new List<string> { "50" };
        var ruleTriggerType = RuleTriggerType.AbsoluteLessThanOrEqual;
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
        var inputValues = new List<string> { "100", "200" };
        var conditionValues = new List<string>();
        var ruleTriggerType = RuleTriggerType.AbsoluteLessThanOrEqual;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

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
        var inputValues = new List<string> { "abc", "200" };
        var conditionValues = new List<string> { "50" };
        var ruleTriggerType = RuleTriggerType.AbsoluteLessThanOrEqual;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeFalse();
        errorLevel.Should().Be(InputErrorLevel.正常);
    }

    [Fact]
    public void 判定値が数値でない_対象外()
    {
        // Arrange
        var inputValues = new List<string> { "100", "200" };
        var conditionValues = new List<string> { "abc" };
        var ruleTriggerType = RuleTriggerType.AbsoluteLessThanOrEqual;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeFalse();
        errorLevel.Should().Be(InputErrorLevel.正常);
    }
}
