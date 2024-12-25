using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査実施判断ルール_検査項目明細
/// </summary>
public class DecisionRuleExamItemDetail
{
    /// <summary>
    /// 変数番号
    /// </summary>
    public required int VariableNumber { get; init; }

    /// <summary>
    /// データソース種別
    /// </summary>
    public required SourceType SourceType { get; init; }

    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public required int ExamItemDetailId { get; init; }
}
