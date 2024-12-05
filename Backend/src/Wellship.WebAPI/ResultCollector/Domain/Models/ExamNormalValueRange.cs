using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査正常値範囲
/// </summary>
public class ExamNormalValueRange
{
    /// <summary>
    /// エラーレベル
    /// </summary>
    public required InputErrorLevel ErrorLevel { get; init; }

    /// <summary>
    /// 最大値
    /// </summary>
    public required int MaxValue { get; init; }

    /// <summary>
    /// 最小値
    /// </summary>
    public required int MinValue { get; init; }

}