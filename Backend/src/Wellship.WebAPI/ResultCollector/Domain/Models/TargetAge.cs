namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 対象年齢範囲
/// </summary>
public class TargetAge
{
    private readonly Age _minAge;
    private readonly Age _maxAge;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="minAgeString">年齢下限文字列（5-7文字）</param>
    /// <param name="maxAgeString">年齢上限文字列（5-7文字）</param>
    public TargetAge(string minAgeString, string maxAgeString)
    {
        // Ageに変換する
        _minAge = new Age(minAgeString);
        _maxAge = new Age(maxAgeString);
    }

    /// <summary>
    /// 対象年齢か判断する
    /// </summary>
    /// <param name="age">年齢</param>
    public bool IsMatch(Age age)
    {
        int ageValue = age.Years * 10000 + age.Months * 100 + age.Days;
        int minValue = _minAge.Years * 10000 + _minAge.Months * 100 + _minAge.Days;
        int maxValue = _maxAge.Years * 10000 + _maxAge.Months * 100 + _maxAge.Days;

        if (minValue <= ageValue && ageValue < maxValue)
        {
            // 年齢下限以上 かつ 年齢上限未満は対象
            // minValue <= ageValue < maxValue
            return true;
        }
        return false;
    }
}
