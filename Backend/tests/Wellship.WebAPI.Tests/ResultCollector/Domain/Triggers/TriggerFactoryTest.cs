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
}
