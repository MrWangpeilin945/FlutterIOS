namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 年齢・月齢・日齢
/// </summary>
public class Age
{
    /// <summary>
    /// 年齢
    /// </summary>
    public int Years { get; }

    /// <summary>
    /// 月齢
    /// </summary>
    public int Months { get; }

    /// <summary>
    /// 日齢
    /// </summary>
    public int Days { get; }

    /// <summary>
    /// 文字列からAgeオブジェクトを生成するコンストラクタ
    /// </summary>
    /// <param name="ageString">年齢文字列</param>
    public Age(string ageString)
    {
        if (string.IsNullOrWhiteSpace(ageString))
        {
            throw new ArgumentNullException(nameof(ageString), "年齢文字列がnullまたは空白です。");
        }

        if (ageString.Length < 5 || ageString.Length > 7)
        {
            throw new ArgumentException("年齢文字列は5文字以上7文字以下で設定してください。", nameof(ageString));
        }

        string paddedAgeString = ageString.PadLeft(7, '0');

        if (!int.TryParse(paddedAgeString.AsSpan(0, 3), out int years) ||
            !int.TryParse(paddedAgeString.AsSpan(3, 2), out int months) ||
            !int.TryParse(paddedAgeString.AsSpan(5, 2), out int days))
        {
            throw new ArgumentException("年齢文字列に無効な数値が含まれています。", nameof(ageString));
        }

        Years = years;
        Months = months;
        Days = days;
    }

    /// <summary>
    /// 年、月、日からAgeオブジェクトを生成するコンストラクタ
    /// </summary>
    /// <param name="years">年齢</param>
    /// <param name="months">月齢</param>
    /// <param name="days">日齢</param>
    public Age(int years, int months, int days)
    {
        if (years < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(years), "年齢は0以上でなければなりません。");
        }

        if (months < 0 || months > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(months), "月齢は0から12の範囲でなければなりません。");
        }

        if (days < 0 || days > 31)
        {
            throw new ArgumentOutOfRangeException(nameof(days), "日齢は0から31の範囲でなければなりません。");
        }

        Years = years;
        Months = months;
        Days = days;
    }
}
