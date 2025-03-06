using FluentAssertions;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models.Triggers;

public class AnyStringNotEqualTest
{
    [Fact]
    public void 入力内容が異なる_対象()
    {
        // Arrange
        var inputValues = new List<string> { "100", "200", "100" };
        var ruleTriggerType = RuleTriggerType.AnyStringNotEqual;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, new List<string>(), InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeTrue();
        errorLevel.Should().Be(InputErrorLevel.異常);
    }

    [Fact]
    public void 入力内容がすべて等しい_対象外()
    {
        // Arrange
        var inputValues = new List<string> { "100", "100", "100" };
        var ruleTriggerType = RuleTriggerType.AnyStringNotEqual;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, new List<string>(), InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeFalse();
        errorLevel.Should().Be(InputErrorLevel.正常);
    }

    [Fact]
    public void 入力内容が1つしかない_対象外()
    {
        // Arrange
        var inputValues = new List<string> { "100" };
        var ruleTriggerType = RuleTriggerType.AnyStringNotEqual;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, new List<string>(), InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeFalse();
        errorLevel.Should().Be(InputErrorLevel.正常);
    }

    [Fact]
    public void 入力内容が空_対象外()
    {
        // Arrange
        var inputValues = new List<string>();
        var ruleTriggerType = RuleTriggerType.AnyStringNotEqual;
        var trigger = TriggerFactory.CreateTrigger(ruleTriggerType, inputValues, new List<string>(), InputErrorLevel.異常);

        // Act
        var matchResult = trigger.IsMatch();
        var errorLevel = trigger.GetErrorLevel();

        // Assert
        matchResult.Should().BeFalse();
        errorLevel.Should().Be(InputErrorLevel.正常);
    }
}
