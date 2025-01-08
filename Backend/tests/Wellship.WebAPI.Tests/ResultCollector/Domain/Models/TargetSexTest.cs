using FluentAssertions;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models;

public class TargetSexTest
{
    [Theory]
    [InlineData(TargetSexType.両方, Sex.男, true)]
    [InlineData(TargetSexType.両方, Sex.女, true)]
    [InlineData(TargetSexType.男, Sex.男, true)]
    [InlineData(TargetSexType.男, Sex.女, false)]
    [InlineData(TargetSexType.女, Sex.女, true)]
    [InlineData(TargetSexType.女, Sex.男, false)]
    public void 対象性別と性別の関係(TargetSexType targetSexType, Sex sex, bool expectedResult)
    {
        // Arrange
        var targetSex = new TargetSex(targetSexType);

        // Act
        var result = targetSex.IsMatch(sex);

        // Assert
        result.Should().Be(expectedResult);
    }
}
