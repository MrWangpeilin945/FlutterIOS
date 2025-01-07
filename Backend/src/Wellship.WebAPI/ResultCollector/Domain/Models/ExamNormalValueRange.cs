using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査正常値範囲
/// </summary>
public class ExamNormalValueRange
{
    /// <summary>
    /// 名称
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 基準値パターンID
    /// </summary>
    public required Guid ThresholdId { get; init; }

    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public required int ExamItemDetailId { get; init; }

    /// <summary>
    /// 対象者年齢下限
    /// </summary>
    public required string MinAge { get; init; }

    /// <summary>
    /// 対象者年齢上限
    /// </summary>
    public required string MaxAge { get; init; }

    /// <summary>
    /// 対象者性別
    /// </summary>
    public required TargetSex TargetSex { get; init; }

    /// <summary>
    /// 最大値
    /// </summary>
    public required decimal MaxValue { get; init; }

    /// <summary>
    /// 最小値
    /// </summary>
    public required decimal MinValue { get; init; }

    /// <summary>
    /// エラーレベル
    /// </summary>
    public required InputErrorLevel ErrorLevel { get; init; }

    /// <summary>
    /// 優先度
    /// </summary>
    public required int Priority { get; init; }

    /// <summary>
    /// 対象年齢範囲かを判断する
    /// </summary>
    public bool IsTargetAge(Age age)
    {
        if (string.IsNullOrWhiteSpace(MinAge) || string.IsNullOrWhiteSpace(MaxAge) ||
           MinAge.Length < 5 || MinAge.Length > 7 || MaxAge.Length < 5 || MaxAge.Length > 7)
        {
            throw new SystemException("年齢設定に不備があります。");
        }
        string minText = MinAge.PadLeft(7, '0');
        string maxText = MaxAge.PadLeft(7, '0');
        // AGEクラスに変換する
        Age minAgeEntity = new Age
        {
            Years = int.Parse(minText.Substring(0, 3)),
            Months = int.Parse(minText.Substring(3, 2)),
            Days = int.Parse(minText.Substring(5, 2))
        };
        Age maxAgeEntity = new Age
        {
            Years = int.Parse(maxText.Substring(0, 3)),
            Months = int.Parse(maxText.Substring(3, 2)),
            Days = int.Parse(maxText.Substring(5, 2))
        };
        int ageValue = age.Years * 10000 + age.Months * 100 + age.Days;
        // 年齢下限
        int minValue = minAgeEntity.Years * 10000 + minAgeEntity.Months * 100 + minAgeEntity.Days;
        // 年齢上限
        int maxValue = maxAgeEntity.Years * 10000 + maxAgeEntity.Months * 100 + maxAgeEntity.Days;
        if (minValue <= ageValue && ageValue < maxValue)
        {
            // 年齢下限以上 かつ 年齢上限未満は対象
            // minValue <= ageValue < maxValue
            return true;
        }
        return false;
    }

    /// <summary>
    /// 対象年齢かを判断する
    /// </summary>
    public bool IsTargetSex(Sex sex)
    {
        return (TargetSex, sex) switch
        {
            (TargetSex.両方, _) => true,
            (TargetSex.男, Sex.男) => true,
            (TargetSex.女, Sex.女) => true,
            _ => false
        };
    }

    /// <summary>
    /// 検査結果値が設定範囲内かどうか判定する
    /// </summary>
    /// <param name="resultText">検査結果値</param>
    public bool ValueInRange(string resultText)
    {
        if (!decimal.TryParse(resultText, out var result))
        {
            return false;
        }

        return MinValue <= result && result < MaxValue;
    }
}