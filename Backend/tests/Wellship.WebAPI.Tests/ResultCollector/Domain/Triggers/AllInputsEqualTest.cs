using FluentAssertions;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models.Triggers;

public class AllInputsNotEqualTest
{
    [Fact]
    public void 入力内容が1つでも異なる_対象()
    {
        // Arrange
        var inputValues = new List<string> { "100", "100", "100", "100", "10", "22" };
        var conditionValues = new List<string>();
        var ruleTriggerType = RuleTriggerType.AllInputsNotEqual;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeTrue();
        errorLevel.Should().Be(InputErrorLevel.異常);
    }

    [Fact]
    public void 入力内容がすべて一致する_対象外()
    {
        // Arrange
        var inputValues = new List<string> { "100", "100", "100", };
        var conditionValues = new List<string>();
        var ruleTriggerType = RuleTriggerType.AllInputsNotEqual;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeFalse();
        errorLevel.Should().Be(InputErrorLevel.正常);
    }

    [Fact]
    public void 入力内容が0件である_対象外()
    {
        // Arrange
        var inputValues = new List<string>();
        var conditionValues = new List<string>();
        var ruleTriggerType = RuleTriggerType.AllInputsNotEqual;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, conditionValues, InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeFalse();
        errorLevel.Should().Be(InputErrorLevel.正常);
    }
}
