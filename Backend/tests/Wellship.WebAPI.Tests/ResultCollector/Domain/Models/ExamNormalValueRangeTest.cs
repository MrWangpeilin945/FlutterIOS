using FluentAssertions;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models;

public class ExamNormalValueRangeTests
{
    public static IEnumerable<object[]> AgeExamNormalValueRanges =>
    [
        [new ExamNormalValueRange() {
            Name = "20歳以上40歳未満", ThresholdId = Guid.Parse("3b0274da-ec3c-4fce-8eac-46c178b2e19c"), ExamItemDetailId = 1, MinAge = "200000", MaxAge = "400000", 
            TargetSex = TargetSex.両方, MaxValue = 999, MinValue = 0, ErrorLevel = InputErrorLevel.正常, Priority = 1
        }, new Age { Years = 40, Months = 0, Days = 0 }, false],        //40歳0ヶ月0日
        [new ExamNormalValueRange() {
            Name = "41歳以上75歳以下", ThresholdId = Guid.Parse("3b0274da-ec3c-4fce-8eac-46c178b2e19c"), ExamItemDetailId = 1, MinAge = "410000", MaxAge = "750001", 
            TargetSex = TargetSex.両方, MaxValue = 999, MinValue = 0, ErrorLevel = InputErrorLevel.正常, Priority = 1
        }, new Age { Years = 75, Months = 0, Days = 0 }, true],         //75歳0ヶ月0日
        [new ExamNormalValueRange() {
            Name = "40歳未満", ThresholdId = Guid.Parse("3b0274da-ec3c-4fce-8eac-46c178b2e19c"), ExamItemDetailId = 1, MinAge = "00000", MaxAge = "400000", 
            TargetSex = TargetSex.両方, MaxValue = 999, MinValue = 0, ErrorLevel = InputErrorLevel.正常, Priority = 1
        }, new Age { Years = 40, Months = 0, Days = 0 }, false],        //40歳0ヶ月0日
        [new ExamNormalValueRange() {
            Name = "40歳未満", ThresholdId = Guid.Parse("3b0274da-ec3c-4fce-8eac-46c178b2e19c"), ExamItemDetailId = 1, MinAge = "00000", MaxAge = "400000", 
            TargetSex = TargetSex.両方, MaxValue = 999, MinValue = 0, ErrorLevel = InputErrorLevel.正常, Priority = 1
        }, new Age { Years = 39, Months = 11, Days = 30 }, true],       //39歳11ヶ月30日
        [new ExamNormalValueRange() {
            Name = "年齢制限なし", ThresholdId = Guid.Parse("3b0274da-ec3c-4fce-8eac-46c178b2e19c"), ExamItemDetailId = 1, MinAge = "00000", MaxAge = "9999999", 
            TargetSex = TargetSex.両方, MaxValue = 999, MinValue = 0, ErrorLevel = InputErrorLevel.正常, Priority = 1
        }, new Age { Years = 0, Months = 0, Days = 0  }, true],         //0歳0ヶ月0日0
        [new ExamNormalValueRange() {
            Name = "年齢制限なし", ThresholdId = Guid.Parse("3b0274da-ec3c-4fce-8eac-46c178b2e19c"), ExamItemDetailId = 1, MinAge = "00000", MaxAge = "9999999", 
            TargetSex = TargetSex.両方, MaxValue = 999, MinValue = 0, ErrorLevel = InputErrorLevel.正常, Priority = 1
        }, new Age { Years = 100, Months = 11, Days = 20 }, true],      //100歳11ヶ月20日
        [new ExamNormalValueRange() {
            Name = "39歳以上42歳未満", ThresholdId = Guid.Parse("3b0274da-ec3c-4fce-8eac-46c178b2e19c"), ExamItemDetailId = 1, MinAge = "390000", MaxAge = "420000", 
            TargetSex = TargetSex.両方, MaxValue = 999, MinValue = 0, ErrorLevel = InputErrorLevel.正常, Priority = 1
        }, new Age { Years = 42, Months = 0, Days = 0 }, false],        //42歳0ヶ月0日
        [new ExamNormalValueRange() {
            Name = "39歳以上42歳以下", ThresholdId = Guid.Parse("3b0274da-ec3c-4fce-8eac-46c178b2e19c"), ExamItemDetailId = 1, MinAge = "390000", MaxAge = "420001", 
            TargetSex = TargetSex.両方, MaxValue = 999, MinValue = 0, ErrorLevel = InputErrorLevel.正常, Priority = 1
        }, new Age { Years = 42, Months = 0, Days = 0 }, true],         //42歳0ヶ月0日
    ];

    public static IEnumerable<object[]> SexExamNormalValueRanges =>
    [
        [new ExamNormalValueRange() {
            Name = "両方:男性", ThresholdId = Guid.Parse("3b0274da-ec3c-4fce-8eac-46c178b2e19c"), ExamItemDetailId = 1, MinAge = "200000", MaxAge = "400000", 
            TargetSex = TargetSex.両方, MaxValue = 999, MinValue = 0, ErrorLevel = InputErrorLevel.正常, Priority = 1
        }, Sex.男, true],
        [new ExamNormalValueRange() {
            Name = "両方:女性", ThresholdId = Guid.Parse("3b0274da-ec3c-4fce-8eac-46c178b2e19c"), ExamItemDetailId = 1, MinAge = "200000", MaxAge = "400000", 
            TargetSex = TargetSex.両方, MaxValue = 999, MinValue = 0, ErrorLevel = InputErrorLevel.正常, Priority = 1
        }, Sex.女, true],
        [new ExamNormalValueRange() {
            Name = "男性:男性", ThresholdId = Guid.Parse("3b0274da-ec3c-4fce-8eac-46c178b2e19c"), ExamItemDetailId = 1, MinAge = "200000", MaxAge = "400000", 
            TargetSex = TargetSex.男, MaxValue = 999, MinValue = 0, ErrorLevel = InputErrorLevel.正常, Priority = 1
        }, Sex.男, true],
        [new ExamNormalValueRange() {
            Name = "男性:女性", ThresholdId = Guid.Parse("3b0274da-ec3c-4fce-8eac-46c178b2e19c"), ExamItemDetailId = 1, MinAge = "200000", MaxAge = "400000", 
            TargetSex = TargetSex.男, MaxValue = 999, MinValue = 0, ErrorLevel = InputErrorLevel.正常, Priority = 1
        }, Sex.女, false],
        [new ExamNormalValueRange() {
            Name = "女性:男性", ThresholdId = Guid.Parse("3b0274da-ec3c-4fce-8eac-46c178b2e19c"), ExamItemDetailId = 1, MinAge = "200000", MaxAge = "400000", 
            TargetSex = TargetSex.女, MaxValue = 999, MinValue = 0, ErrorLevel = InputErrorLevel.正常, Priority = 1
        }, Sex.男, false],
        [new ExamNormalValueRange() {
            Name = "女性:女性", ThresholdId = Guid.Parse("3b0274da-ec3c-4fce-8eac-46c178b2e19c"), ExamItemDetailId = 1, MinAge = "200000", MaxAge = "400000", 
            TargetSex = TargetSex.女, MaxValue = 999, MinValue = 0, ErrorLevel = InputErrorLevel.正常, Priority = 1
        }, Sex.女, true],
    ];

    [Theory]
    [MemberData(nameof(AgeExamNormalValueRanges))]
    public void 検査正常値範囲の対象年齢を判定する(ExamNormalValueRange examNormalValueRange, Age age, bool expected)
    {
        // Act
        var actual = examNormalValueRange.IsTargetAge(age);

        // Assert
        actual.Should().Be(expected);
    }

        [Theory]
    [MemberData(nameof(SexExamNormalValueRanges))]
    public void 検査正常値範囲の対象性別を判定する(ExamNormalValueRange examNormalValueRange, Sex sex, bool expected)
    {
        // Act
        var actual = examNormalValueRange.IsTargetSex(sex);

        // Assert
        actual.Should().Be(expected);
    }

}
