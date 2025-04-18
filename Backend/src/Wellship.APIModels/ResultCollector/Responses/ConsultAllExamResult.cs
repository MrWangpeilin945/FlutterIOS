using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 受診者の全ての検査結果
/// </summary>
public class ConsultAllExamResult
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
    /// 検査結果
    /// </summary>
    [JsonPropertyName("examResults")]
    public required IEnumerable<DisplayExamResultMenu> ExamResults { get; init; }
}