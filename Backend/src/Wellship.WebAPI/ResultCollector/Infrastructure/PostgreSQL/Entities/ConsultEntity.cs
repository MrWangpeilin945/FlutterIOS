namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 受診
/// </summary>
public class ConsultEntity
{
    /// <summary>
    /// 受診者ID
    /// </summary>
    public required int ConsultId { get; init; }

    /// <summary>
    /// 受診番号
    /// </summary>
    public required string ConsultNumber { get; init; }

    /// <summary>
    /// 受診進捗状況
    /// </summary>
    public required int ProgressStatus { get; init; }

    /// <summary>
    /// 受診データ出力状況
    /// </summary>
    public required int ExportStatus { get; init; }

    /// <summary>
    /// 会場日程ID
    /// </summary>
    public required int PlaceScheduleId { get; init; }

    /// <summary>
    /// 受診者ID
    /// </summary>
    public required int ExamineeId { get; init; }
}
