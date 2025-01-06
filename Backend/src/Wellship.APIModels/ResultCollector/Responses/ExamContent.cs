using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査内容
/// </summary>
public class ExamContent
{
    /// <summary>
    /// 受診番号
    /// </summary>
    [JsonPropertyName("consultNumber")]
    public required string ConsultNumber { get; init; }

    /// <summary>
    /// 特記事項
    /// </summary>
    [JsonPropertyName("consultName")]
    public required string ConsultName { get; init; }

    /// <summary>
    /// 受診者情報
    /// </summary>
    [JsonPropertyName("examinee")]
    public required Examinee Examinee { get; init; }

    /// <summary>
    /// 検査実施判断
    /// </summary>
    [JsonPropertyName("isComplete")]
    public required bool IsComplete { get; init; }

    /// <summary>
    /// 関連検査項目
    /// </summary>
    [JsonPropertyName("relatedExamItems")]
    public required RelatedExamItem[] RelatedExamItems { get; init; }

    /// <summary>
    /// 実施検査項目
    /// </summary>
    [JsonPropertyName("examItems")]
    public required ExamDetail[] ExamItems { get; init; }

    /// <summary>
    /// 検査実施判断結果
    /// </summary>
    [JsonPropertyName("examDecisionResults")]
    public required ExamDecisionResult[] ExamDecisionResults { get; init; }

    /// <summary>
    /// 未実施検査項目
    /// </summary>
    [JsonPropertyName("unexaminedItems")]
    public required ExamMenu[] UnexaminedItems { get; init; }

}
