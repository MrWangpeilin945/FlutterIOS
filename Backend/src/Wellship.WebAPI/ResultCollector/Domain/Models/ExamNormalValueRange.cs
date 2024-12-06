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
    public required int ThresholdId { get; init; }

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
    public required Sex TargetSex { get; init; }

    /// <summary>
    /// 最大値
    /// </summary>
    public required int MaxValue { get; init; }

    /// <summary>
    /// 最小値
    /// </summary>
    public required int MinValue { get; init; }

    /// <summary>
    /// エラーレベル
    /// </summary>
    public required InputErrorLevel ErrorLevel { get; init; }

    /// <summary>
    /// 優先度
    /// </summary>
    public required int Priority { get; init; }

}