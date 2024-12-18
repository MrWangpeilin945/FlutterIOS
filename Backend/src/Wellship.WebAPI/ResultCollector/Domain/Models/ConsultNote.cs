namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 受診に紐づく特記事項
/// </summary>
public class ConsultNote
{
    /// <summary>
    /// 検査特記コード
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// 特記事項
    /// </summary>
    public required string Note { get; init; }
}
