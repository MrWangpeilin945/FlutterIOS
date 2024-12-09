using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 生年月日
/// </summary>
public sealed class TargetAge
{
    /// <summary>
    /// 最小年齢
    /// </summary>
    public Age MinAge { get; } 
    
    /// <summary>
    /// 最大年齢
    /// </summary>
    public Age MaxAge { get; }
    
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public TargetAge(string minAge, string maxAge)
    {
        if(string.IsNullOrWhiteSpace(minAge) || string.IsNullOrWhiteSpace(maxAge) || 
           minAge.Length < 5 || minAge.Length > 7 || maxAge.Length < 5 || maxAge.Length > 7 )
        {
            throw new SystemException("年齢設定に不備があります。");
        }
        string minText = minAge.PadLeft(7, '0');
        string maxText = maxAge.PadLeft(7, '0');
        // AGEクラスに変換する
        MinAge = new Age{
            Years = int.Parse(minText.Substring(0, 3)),
            Months = int.Parse(minText.Substring(3, 2)),
            Days = int.Parse(minText.Substring(5, 2))
        };
        MaxAge  = new Age{
            Years = int.Parse(maxText.Substring(0, 3)),
            Months = int.Parse(maxText.Substring(3, 2)),
            Days = int.Parse(maxText.Substring(5, 2))
        };
    }

    /// <summary>
    /// 対象年齢範囲かを判断する
    /// </summary>
    /// <param name="age"></param>
    public bool IsTargetAge(Age age)
    {
        int ageValue = age.Years * 10000 + age.Months * 100 + age.Days;
        int minValue = MinAge.Years * 10000 + MinAge.Months * 100 + MinAge.Days;
        if (ageValue < minValue)
        {
            return false;
        }
        int maxValue = MaxAge.Years * 10000 + MaxAge.Months * 100 + MaxAge.Days;
        if (ageValue > maxValue)
        {
            return false;
        }
        return true;
    }
}
