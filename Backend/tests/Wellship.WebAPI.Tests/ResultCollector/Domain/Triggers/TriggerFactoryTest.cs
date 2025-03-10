using FluentAssertions;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models.Triggers;

public class TriggerFactoryTest
{
    [Fact]
    public void AllInputsEqualを生成する()
    {
        // Arrange
        var inputValues = new List<string> { "100", "100", "100", "100" };
        var conditionValues = new List<string>();

        // Act
        var actual = TriggerFactory.CreateTrigger(RuleTriggerType.AllInputsNotEqual, inputValues, conditionValues, InputErrorLevel.異常);

        // Assert
        actual.Should().BeOfType<AllInputsNotEqual>();
    }

    [Fact]
    public void RuleTriggerTypeで定義されていない値のとき例外をスローする()
    {
        // Arrange
        var inputValues = new List<string> { "100", "200" };
        var conditionValues = new List<string> { "50" };
        var errorLevel = InputErrorLevel.異常;
        var invalidTriggerType = (RuleTriggerType)9999; // 意図しない値

        // Act
        Action act = () => TriggerFactory.CreateTrigger(invalidTriggerType, inputValues, conditionValues, errorLevel);

        // Assert
        act.Should().Throw<NotImplementedException>();
    }
}
