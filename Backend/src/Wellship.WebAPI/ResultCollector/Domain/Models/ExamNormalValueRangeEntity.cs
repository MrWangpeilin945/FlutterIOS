namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査基準値範囲のエンティティ
/// </summary>
public class ExamNormalValueRangeEntity
{
    /// <summary>
    /// 受診番号
    /// </summary>
    public required Guid ConsultId { get; set; }

    /// <summary>
    /// 受診番号
    /// </summary>
    public required string ConsultNumber { get; set; }

    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public required int ExamItemDetailId { get; set; }

    /// <summary>
    /// 優先度
    /// </summary>
    public required int Priority { get; set; }

    /// <summary>
    /// 基準値パターンID
    /// </summary>
    public required Guid ThresholdId { get; set; }

    /// <summary>
    /// 基準値パターンコード
    /// </summary>
    public required string ThresholdCode { get; set; }

    /// <summary>
    /// 基準値パターン名
    /// </summary>
    public required string ThresholdName { get; set; }

    /// <summary>
    /// 基準値範囲ID
    /// </summary>
    public required Guid RangeId { get; set; }

    /// <summary>
    /// 基準値範囲名
    /// </summary>
    public required string RangeName { get; set; }

    /// <summary>
    /// 対象年齢下限
    /// </summary>
    public required string MinAge { get; set; }

    /// <summary>
    /// 対象年齢上限
    /// </summary>
    public required string MaxAge { get; set; }

    /// <summary>
    /// 対象性別
    /// </summary>
    public required int TargetSex { get; set; }

    /// <summary>
    /// 値下限
    /// </summary>
    public required decimal MinValue { get; set; }

    /// <summary>
    /// 値上限
    /// </summary>
    public required decimal MaxValue { get; set; }

    /// <summary>
    /// エラーレベル
    /// </summary>
    public required int ErrorLevel { get; set; }
}
