using FluentAssertions;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models.Triggers;

public class StringEqualAndContainedInListTest
{
    [Fact]
    public void 入力1と判定1が等しく入力2がリストに含まれる_対象()
    {
        // Arrange
        var inputValues = new List<string> { "test1", "test2" };
        var conditionValues = new List<string> { "test1", "test2", "test3" };
        var ruleTriggerType = RuleTriggerType.StringEqualAndContainedInList;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeTrue();
        errorLevel.Should().Be(InputErrorLevel.異常);
    }

    [Fact]
    public void 入力1と判定1が等しくない_対象外()
    {
        // Arrange
        var inputValues = new List<string> { "test1", "test2" };
        var conditionValues = new List<string> { "test3", "test2", "test4" };
        var ruleTriggerType = RuleTriggerType.StringEqualAndContainedInList;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeFalse();
        errorLevel.Should().Be(InputErrorLevel.正常);
    }

    [Fact]
    public void 入力2がリストに含まれない_対象外()
    {
        // Arrange
        var inputValues = new List<string> { "test1", "test4" };
        var conditionValues = new List<string> { "test1", "test2", "test3" };
        var ruleTriggerType = RuleTriggerType.StringEqualAndContainedInList;
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
        var inputValues = new List<string> { "test1" };
        var conditionValues = new List<string> { "test1", "test2", "test3" };
        var ruleTriggerType = RuleTriggerType.StringEqualAndContainedInList;
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
        var inputValues = new List<string> { "test1", "test2" };
        var conditionValues = new List<string> { "test1" };
        var ruleTriggerType = RuleTriggerType.StringEqualAndContainedInList;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeFalse();
        errorLevel.Should().Be(InputErrorLevel.正常);
    }
}