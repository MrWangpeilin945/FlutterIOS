using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 受診に紐づく受診者情報
/// </summary>
public class ConsultExaminee
{
    /// <summary>
    /// 受診番号
    /// </summary>
    public required string ConsultNumber { get; init; }

    /// <summary>
    /// 受付番号
    /// </summary>
    public string? TicketNumber { get; init; }

    /// <summary>
    /// カナ氏名
    /// </summary>
    public required string KanaName { get; init; }

    /// <summary>
    /// 性別
    /// </summary>
    public required Sex Sex { get; init; }

    /// <summary>
    /// 受付日時
    /// </summary>
    public DateTimeOffset? CheckedInAt { get; init; }
}