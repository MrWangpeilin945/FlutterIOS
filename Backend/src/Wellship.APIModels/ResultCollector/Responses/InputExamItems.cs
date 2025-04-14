using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査結果入力情報
/// </summary>
public class InputExamItems
{
    /// <summary>
    /// 受診番号
    /// </summary>
    [JsonPropertyName("consultNumber")]
    public required string ConsultNumber { get; init; }

    /// <summary>
    /// 受診者情報
    /// </summary>
    [JsonPropertyName("examinee")]
    public required InputExamExaminee Examinee { get; init; }

    /// <summary>
    /// 関連検査項目
    /// </summary>
    [JsonPropertyName("relatedExamItems")]
    public required RelatedExamItem[] RelatedExamItems { get; init; }

    /// <summary>
    /// 関連検査項目
    /// </summary>
    [JsonPropertyName("isComplete")]
    public required bool IsComplete { get; init; }

    /// <summary>
    /// 検査結果入力項目グループ
    /// </summary>
    [JsonPropertyName("examItemGroups")]
    public required ExamItemGroup[] ExamItemGroups { get; init; }

}
