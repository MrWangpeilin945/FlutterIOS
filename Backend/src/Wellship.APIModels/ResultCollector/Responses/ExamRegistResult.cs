using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査結果登録エラー
/// </summary>
public class ExamRegistResult
{
    /// <summary>
    /// エラーレベル
    /// </summary>
    [JsonPropertyName("errorLevel")]
    public required int ErrorLevel { get; init; }

    /// <summary>
    /// エラー内容
    /// </summary>
    [JsonPropertyName("description")]
    public required string Description { get; init; }
}
