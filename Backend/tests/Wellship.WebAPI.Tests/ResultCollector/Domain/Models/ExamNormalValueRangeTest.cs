using FluentAssertions;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models;

public class ExamNormalValueRangeTests
{
    [Fact]
    public void 検査値が範囲内である_数値()
    {
        // Arrange
        var examNormalValueRange = new ExamNormalValueRange(name: "0以上4未満",
                                                            thresholdId: Guid.NewGuid(),
                                                            examItemDetailId: 2,
                                                            targetAge: new TargetAge("00000", "9999999"),
                                                            targetSex: new TargetSex(TargetSexType.両方),
                                                            valueRange: new ValueRange(0, 4),
                                                            errorLevel: InputErrorLevel.警告,
                                                            priority: 1);

        // Act
        var result = examNormalValueRange.ValueRange.InRange("3.9999");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void 検査値が範囲内でない_数値()
    {
        // Arrange
        var examNormalValueRange = new ExamNormalValueRange(name: "0以上4未満",
                                                            thresholdId: Guid.NewGuid(),
                                                            examItemDetailId: 2,
                                                            targetAge: new TargetAge("00000", "9999999"),
                                                            targetSex: new TargetSex(TargetSexType.両方),
                                                            valueRange: new ValueRange(0, 4),
                                                            errorLevel: InputErrorLevel.警告,
                                                            priority: 1);

        // Act
        var result = examNormalValueRange.ValueRange.InRange("4.0001");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void 基準値範囲_数値でない空文字()
    {
        // Arrange
        var examNormalValueRange = new ExamNormalValueRange(name: "0以上4未満",
                                                            thresholdId: Guid.NewGuid(),
                                                            examItemDetailId: 2,
                                                            targetAge: new TargetAge("00000", "9999999"),
                                                            targetSex: new TargetSex(TargetSexType.両方),
                                                            valueRange: new ValueRange(0, 4),
                                                            errorLevel: InputErrorLevel.警告,
                                                            priority: 1);


        // Act
        var result = examNormalValueRange.ValueRange.InRange("");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void 基準値範囲_数値でない文字列()
    {
        // Arrange
        var examNormalValueRange = new ExamNormalValueRange(name: "0以上4未満",
                                                            thresholdId: Guid.NewGuid(),
                                                            examItemDetailId: 2,
                                                            targetAge: new TargetAge("00000", "9999999"),
                                                            targetSex: new TargetSex(TargetSexType.両方),
                                                            valueRange: new ValueRange(0, 4),
                                                            errorLevel: InputErrorLevel.警告,
                                                            priority: 1);

        // Act
        var result = examNormalValueRange.ValueRange.InRange("文字列");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void 基準値範囲のオブジェクト生成()
    {
        // Arrange
        var name = "0以上4未満";
        var thresholdId = Guid.Parse("6fcdd849-e604-4912-b6f9-91733f280706");
        var examItemDetailId = 2;
        var targetAge = new TargetAge("00000", "9999999");
        var targetSex = new TargetSex(TargetSexType.両方);
        var valueRange = new ValueRange(0, 4);
        var errorLevel = InputErrorLevel.警告;
        var priority = 1;

        // Act
        var examNormalValueRange = new ExamNormalValueRange(name: name,
                                                            thresholdId: thresholdId,
                                                            examItemDetailId: examItemDetailId,
                                                            targetAge: targetAge,
                                                            targetSex: targetSex,
                                                            valueRange: valueRange,
                                                            errorLevel: errorLevel,
                                                            priority: priority);

        // Assert
        examNormalValueRange.Name.Should().Be(name);
        examNormalValueRange.ThresholdId.Should().Be(thresholdId);
        examNormalValueRange.TargetAge.Should().Be(targetAge);
        examNormalValueRange.TargetSex.Should().Be(targetSex);
        examNormalValueRange.ValueRange.Should().Be(valueRange);
        examNormalValueRange.ErrorLevel.Should().Be(errorLevel);
        examNormalValueRange.Priority.Should().Be(priority);
    }
}
