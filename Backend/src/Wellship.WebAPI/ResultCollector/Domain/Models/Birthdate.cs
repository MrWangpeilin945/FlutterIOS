using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 生年月日
/// </summary>
public sealed class Birthdate
{
    /// <summary>
    /// DateOnlyから生成します。
    /// </summary>
    public Birthdate(DateOnly dateOnly)
    {
        Value = dateOnly;
    }

    /// <summary>
    /// 文字列（yyyyMMdd）から生成する
    /// </summary>
    public Birthdate(string birhdateString)
    {
        if (DateOnly.TryParseExact(birhdateString, "yyyyMMdd", out var dateOnly))
        {
            Value = dateOnly;
        }
    }

    /// <summary>
    /// 文字列にします。
    /// </summary>
    public override string ToString()
    {
        return Value.ToString("yyyyMMdd");
    }

    /// <summary>
    /// 生年月日を取得します。
    /// </summary>
    public DateOnly Value { get; }

    /// <summary>
    /// 年齢を取得します。
    /// </summary>
    /// <param name="startDate">起算日</param>
    /// <param name="mode">年齢計算モード（加算方法）</param>
    public Age GetAge(DateOnly startDate, AgeCalcMode mode)
    {
        // TODO: ここに年齢計算ロジックを書く
        return new Age()
        {
            Years = 40,
            Months = 0,
            Days = 0
        };
    }
}
