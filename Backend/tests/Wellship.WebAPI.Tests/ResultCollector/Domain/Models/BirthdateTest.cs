using FluentAssertions;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models;

public class BirthdateTests
{
    public static IEnumerable<object[]> Birthdates =>
    [
        [new Birthdate("19800101"), new DateOnly(2020, 1, 1), AgeCalcMode.当日年齢加算, new Age { Years = 40, Months = 0, Days = 0 }],
        [new Birthdate("19800101"), new DateOnly(2024, 1, 1), AgeCalcMode.当日年齢加算, new Age { Years = 44, Months = 0, Days = 0 }]
    ];

    [Theory]
    [MemberData(nameof(Birthdates))]
    public void 年齢を計算する(Birthdate birthdate, DateOnly calcDate, AgeCalcMode calculationMode, Age expected)
    {
        // Act
        var actual = birthdate.GetAge(calcDate, calculationMode);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}
