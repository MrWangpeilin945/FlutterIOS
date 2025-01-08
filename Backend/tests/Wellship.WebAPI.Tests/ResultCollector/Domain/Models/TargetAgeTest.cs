using FluentAssertions;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models;

public class TargetAgeTest
{
    [Fact]
    public void 設定された年齢文字列が空文字_例外送出()
    {
        // Arrange
        Action act = () => new TargetAge("", "");

        // Act & Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("年齢文字列がnullまたは空白です。*");
    }

    [Fact]
    public void 設定された年齢文字列が5文字未満_例外送出()
    {
        // Arrange
        Action act = () => new TargetAge("000", "999");

        // Act & Assert
        act.Should().Throw<ArgumentException>().WithMessage("年齢文字列は5文字以上7文字以下で設定してください。*");
    }

    [Fact]
    public void 設定された年齢文字列が8文字以上_例外送出()
    {
        // Arrange
        Action act = () => new TargetAge("00000000", "99999999");

        // Act & Assert
        act.Should().Throw<ArgumentException>().WithMessage("年齢文字列は5文字以上7文字以下で設定してください。*");
    }

    // NOTE: 年齢範囲の条件は Min以上 Max未満です。

    [Fact]
    public void 下限範囲_境界外_false()
    {
        // Arrange
        var targetRange = new TargetAge("0420600", "0500000");
        var age = new Age() { Years = 42, Months = 5, Days = 31 };

        // Act
        var result = targetRange.IsMatch(age);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void 下限範囲_境界内_true()
    {
        // Arrange
        var targetRange = new TargetAge("0420600", "0500000");
        var age = new Age() { Years = 42, Months = 6, Days = 0 };

        // Act
        var result = targetRange.IsMatch(age);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void 上限範囲_境界内_true()
    {
        // Arrange
        var targetRange = new TargetAge("0420600", "0500000");
        var age = new Age() { Years = 49, Months = 12, Days = 31 };

        // Act
        var result = targetRange.IsMatch(age);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void 上限範囲_境界外_false()
    {
        // Arrange
        var targetRange = new TargetAge("0420600", "0500000");
        var age = new Age() { Years = 50, Months = 0, Days = 0 };

        // Act
        var result = targetRange.IsMatch(age);

        // Assert
        result.Should().BeFalse();
    }
}
