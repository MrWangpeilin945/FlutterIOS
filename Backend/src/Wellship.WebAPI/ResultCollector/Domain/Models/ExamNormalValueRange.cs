using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査基準値範囲
/// </summary>
public class ExamNormalValueRange
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ExamNormalValueRange(ExamNormalValueRangeEntity entity)
    {
        Name = entity.RangeName;
        ThresholdId = entity.ThresholdId;
        ExamItemDetailId = entity.ExamItemDetailId;
        TargetAge = new TargetAge(entity.MinAge, entity.MaxAge);
        TargetSex = new TargetSex((TargetSexType)entity.TargetSex);
        ValueRange = new ValueRange(entity.MinValue, entity.MaxValue);
        ErrorLevel = (InputErrorLevel)entity.ErrorLevel;
        Priority = entity.Priority;
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ExamNormalValueRange(string name,
                                Guid thresholdId,
                                int examItemDetailId,
                                TargetAge targetAge,
                                TargetSex targetSex,
                                ValueRange valueRange,
                                InputErrorLevel errorLevel,
                                int priority)
    {
        Name = name;
        ThresholdId = thresholdId;
        ExamItemDetailId = examItemDetailId;
        TargetAge = targetAge;
        TargetSex = targetSex;
        ValueRange = valueRange;
        ErrorLevel = errorLevel;
        Priority = priority;
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 基準値パターンID
    /// </summary>
    public Guid ThresholdId { get; }

    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public int ExamItemDetailId { get; }

    /// <summary>
    /// 対象年齢
    /// </summary>
    public TargetAge TargetAge { get; }

    /// <summary>
    /// 対象者性別
    /// </summary>
    public TargetSex TargetSex { get; }

    /// <summary>
    /// 値の範囲
    /// </summary>
    public ValueRange ValueRange { get; }

    /// <summary>
    /// エラーレベル
    /// </summary>
    public InputErrorLevel ErrorLevel { get; }

    /// <summary>
    /// 優先度
    /// </summary>
    public int Priority { get; }
}
