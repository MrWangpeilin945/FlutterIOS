namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 受診に紐づく受診者情報のエンティティ
/// </summary>
public class ConsultExamineeEntity
{
    /// <summary>
    /// 受診番号
    /// </summary>
    public required string ConsultNumber { get; set; }

    /// <summary>
    /// 受付番号
    /// </summary>
    public string? TicketNumber { get; set; }

    /// <summary>
    /// カナ氏名
    /// </summary>
    public required string KanaName { get; set; }

    /// <summary>
    /// 性別
    /// </summary>
    public required int Sex { get; set; }

    /// <summary>
    /// 受付日時
    /// </summary>
    public DateTimeOffset? CheckedInAt { get; set; }
}