using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査正常値のエラー
/// </summary>
public class RangeError
{
    /// <summary>
    /// 範囲ID
    /// </summary>
    public int RangeId { get; init; }

    /// <summary>
    /// 範囲名称
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public required int ExamItemDetailId { get; init; }

    /// <summary>
    /// 値上限
    /// </summary>
    public required decimal MaxValue { get; init; }

    /// <summary>
    /// 値下限
    /// </summary>
    public required decimal MinValue { get; init; }

    /// <summary>
    /// エラーレベル
    /// </summary>
    public required InputErrorLevel ErrorLevel { get; init; }
}
