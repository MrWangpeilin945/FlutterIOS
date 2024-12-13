namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 受診に紐づく検査項目単位の特記事項
/// </summary>
public class ExamItemNote
{
    /// <summary>
    /// 検査項目ID
    /// </summary>
    public required int ExamItemId { get; init; }

    /// <summary>
    /// 特記事項
    /// </summary>
    public required string Note { get; init; }
}
