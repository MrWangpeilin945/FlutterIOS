using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査実施判断ルールの結果
/// </summary>
public class ExamDecisionResult
{
    /// <summary>
    /// エラーレベル
    /// </summary>
    [JsonPropertyName("errorLevel")]
    public required int ErrorLevel { get; init; }

    /// <summary>
    /// 説明
    /// </summary>
    [JsonPropertyName("description")]
    public required string Description { get; init; }
}
