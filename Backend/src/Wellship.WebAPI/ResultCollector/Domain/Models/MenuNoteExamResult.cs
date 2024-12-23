using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査メニュー特記マスタ_検査結果
/// </summary>
public class MenuNoteExamResult
{
    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public required int ExamItemDetailId { get; init; }

    /// <summary>
    /// データソース種別
    /// </summary>
    public required SourceType SourceType { get; init; }
}
