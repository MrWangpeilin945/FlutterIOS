namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 基準値範囲
/// </summary>
public class ValueRange
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="minValue">値下限</param>
    /// <param name="maxValue">値上限</param>
    public ValueRange(decimal minValue, decimal maxValue)
    {
        MinValue = minValue;
        MaxValue = maxValue;
    }

    /// <summary>
    /// 値下限
    /// </summary>
    public decimal MinValue { get; }

    /// <summary>
    /// 値上限
    /// </summary>
    public decimal MaxValue { get; }

    /// <summary>
    /// 基準値範囲に当てはまるか
    /// </summary>
    /// <param name="valueString">値の文字列</param>
    public bool InRange(string valueString)
    {
        if (!decimal.TryParse(valueString, out var value))
        {
            return false;
        }

        return MinValue <= value && value < MaxValue;
    }
}
