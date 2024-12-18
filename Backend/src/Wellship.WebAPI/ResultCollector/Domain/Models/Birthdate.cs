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
        DateOnly initStartDate = startDate;
        if (mode == AgeCalcMode.前日年齢加算)
        {
            if (startDate == Value)
            {
                // 生年月日に達していないため-1を返す
                return new Age()
                {
                    Years = 0,
                    Months = 0,
                    Days = -1
                };
            }
            // 年齢到達日は前日のため、起算日を１日後に移動する
            startDate = startDate.AddDays(1);
        }
        int monthDiff = (startDate.Month + (startDate.Year - Value.Year) * 12) - Value.Month;
        int years = (int)Math.Floor(monthDiff / 12m);
        int months = monthDiff % 12;
        int days = 0;
        if (Value.Day <= startDate.Day)
        {
            days = startDate.Day - Value.Day;
        }
        else
        {
            // 前月の月末日を求める
            int monthDays = DateTime.DaysInMonth(startDate.AddMonths(-1).Year, startDate.AddMonths(-1).Month);
            // 起算年の誕生日月の月日数を求める
            int birthMonthDays = DateTime.DaysInMonth(startDate.Year, Value.Month);
            int diffDay = monthDays - Value.Day;
            if (Value.Day < birthMonthDays && birthMonthDays < monthDays)
            {
                diffDay = birthMonthDays - Value.Day + 1;
            }
            if (diffDay >= 0 && (Value.Day != birthMonthDays || (mode == AgeCalcMode.前日年齢加算 && startDate.Day == 1)))
            {
                days = diffDay + startDate.Day;
            }
            else
            {
                days = startDate.Day;
            }
            //年月の調整
            if (months > 0)
            {
                // 起算月の日数
                int startMonthDays = DateTime.DaysInMonth(startDate.Year, startDate.Month);
                if (initStartDate.Day == days && startMonthDays == days)
                {
                    // 月年齢を減算せず、日年齢をクリアする
                    days = 0;
                }
                else
                {
                    months -= 1;
                }
            }
            else
            {
                months = 11;
                years -= 1;
            }
        }
        return new Age()
        {
            Years = years,
            Months = months,
            Days = days
        };
    }
}
