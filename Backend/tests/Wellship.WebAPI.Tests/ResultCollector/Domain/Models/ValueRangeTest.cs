using FluentAssertions;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models;

public class ValueRangeTest
{
    // NOTE: 年齢範囲の条件は Min以上 Max未満です。

    [Fact]
    public void 下限範囲_境界外_false()
    {
        // Arrange
        var range = new ValueRange(20.0m, 180.0m);
        var value = 19.99m;

        // Act
        var result = range.InRange(value.ToString());

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void 下限範囲_境界内_true()
    {
        // Arrange
        var range = new ValueRange(20.0m, 180.0m);
        var value = 20.0m;

        // Act
        var result = range.InRange(value.ToString());

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void 上限範囲_境界内_true()
    {
        // Arrange
        var range = new ValueRange(20.0m, 180.0m);
        var value = 179.9m;

        // Act
        var result = range.InRange(value.ToString());

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void 上限範囲_境界外_false()
    {
        // Arrange
        var range = new ValueRange(20.0m, 180.0m);
        var value = 180.0m;

        // Act
        var result = range.InRange(value.ToString());

        // Assert
        result.Should().BeFalse();
    }
}
