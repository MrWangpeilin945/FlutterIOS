using FluentAssertions;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models;

public class TargetAgeTests
{
    public static IEnumerable<object[]> Ages =>
    [
        [new TargetAge("200000", "400000"), new Age { Years = 40, Months = 0, Days = 0 }, false],   //20歳以上40歳未満  40歳
        [new TargetAge("410000", "750001"), new Age { Years = 75, Months = 0, Days = 0 }, true],    //41歳以上75歳以下  75歳
        [new TargetAge("00000", "400000"), new Age { Years = 40, Months = 0, Days = 0 }, false],    //40歳未満          40歳
        [new TargetAge("00000", "400000"), new Age { Years = 39, Months = 11, Days = 30 }, true],   //40歳未満          39歳11ヶ月30日
        [new TargetAge("00000", "9999999"), new Age { Years = 0, Months = 0, Days = 0 }, true],     //年齢制限なし       0歳0ヶ月0日
        [new TargetAge("00000", "9999999"), new Age { Years = 100, Months = 11, Days = 20 }, true], //年齢制限なし     100歳11ヶ月20日
        [new TargetAge("390000", "420000"), new Age { Years = 42, Months = 0, Days = 0 }, false],   //39歳以上42歳未満  42歳0ヶ月0日
        [new TargetAge("390000", "420001"), new Age { Years = 42, Months = 0, Days = 0 }, true],   //39歳以上42歳以下  42歳0ヶ月0日
    ];

    [Theory]
    [MemberData(nameof(Ages))]
    public void 対象年齢を判定する(TargetAge targetAge, Age age, bool expected)
    {
        // Act
        var actual = targetAge.IsTargetAge(age);

        // Assert
        actual.Should().Be(expected);
    }
}
