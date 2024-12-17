using FluentAssertions;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models.Triggers;

public class ThresholdExceededTest
{
    [Fact]
    public void 入力値と条件値が正常な場合_条件を満たす_正()
    {
        // Arrange
        // 差は20.9、しきい値は20
        var inputValues = new List<string>() { "60", "80.9" }; // 入力値を適切に設定
        var conditionValues = new List<string>() { "20" };    // 条件値を適切に設定
        var ruleTriggerType = RuleTriggerType.ThresholdExceeded;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        var expected = InputErrorLevel.異常;

        // Act
        var result = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        result.Should().BeTrue();
        errorLevel.Should().Be(expected);
    }

    [Fact]
    public void 入力値と条件値が正常な場合_条件を満たす_負()
    {
        // Arrange
        // 差は-21、しきい値は20
        var inputValues = new List<string>() { "81", "60" }; // 入力値を適切に設定
        var conditionValues = new List<string>() { "20" };    // 条件値を適切に設定
        var ruleTriggerType = RuleTriggerType.ThresholdExceeded;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        var expected = InputErrorLevel.異常;

        // Act
        var result = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        result.Should().BeTrue();
        errorLevel.Should().Be(expected);
    }

    [Fact]
    public void 入力値と条件値が正常な場合_条件を満たさない()
    {
        // Arrange
        // 差は20、しきい値は20
        var inputValues = new List<string>() { "60", "80" };
        var conditionValues = new List<string>() { "20" };
        var ruleTriggerType = RuleTriggerType.ThresholdExceeded;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var result = trigger.IsMatch();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void 入力値が不正な場合_条件を満たさない()
    {
        // Arrange
        var inputValues = new List<string>() { "invalid", "20" };
        var conditionValues = new List<string>() { "5" };
        var ruleTriggerType = RuleTriggerType.ThresholdExceeded;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var result = trigger.IsMatch();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void 条件値が不正な場合_条件を満たさない()
    {
        // Arrange
        var inputValues = new List<string>() { "10", "20" };
        var conditionValues = new List<string>() { "invalid" };
        var ruleTriggerType = RuleTriggerType.ThresholdExceeded;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var result = trigger.IsMatch();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void 入力値が足りない場合_条件を満たさない()
    {
        // Arrange
        var inputValues = new List<string>() { "10" };
        var conditionValues = new List<string>() { "5" };
        var ruleTriggerType = RuleTriggerType.ThresholdExceeded;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var result = trigger.IsMatch();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void 条件値が足りない場合_条件を満たさない()
    {
        // Arrange
        var inputValues = new List<string>() { "10", "20" };
        var conditionValues = new List<string>();
        var ruleTriggerType = RuleTriggerType.ThresholdExceeded;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var result = trigger.IsMatch();

        // Assert
        result.Should().BeFalse();
    }
}
