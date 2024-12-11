using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査項目グループ
/// </summary>
public class ExamItemGroup
{
    /// <summary>
    /// 検査項目グループID
    /// </summary>
    public required int ExamItemGroupId { get; init; }

    /// <summary>
    /// 検査項目グループ種別
    /// </summary>
    public required ExamItemGroupType Type { get; init; }

    /// <summary>
    /// 検査項目
    /// </summary>
    public required IEnumerable<ExamItem> ExamItems{ get; init; }
}
