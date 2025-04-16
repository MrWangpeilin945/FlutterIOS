namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査項目明細単位の表示用検査結果
/// </summary>

public class DisplayExamResultItemDetail
{
    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public required int ExamItemDetailId { get; init; }

    /// <summary>
    /// 検査項目明細名
    /// </summary>
    public required string ExamItemDetailName { get; init; }

    /// <summary>
    /// 今回値
    /// </summary>
    public string? CurrentResult { get; init; }

    /// <summary>
    /// 過去値
    /// </summary>
    public string? PastResult { get; init; }

    /// <summary>
    /// 過去検査日
    /// </summary>
    public DateOnly? PastDate { get; init; }

    /// <summary>
    /// 直近判定
    /// </summary>
    public required bool IsRecent { get; init; }
}