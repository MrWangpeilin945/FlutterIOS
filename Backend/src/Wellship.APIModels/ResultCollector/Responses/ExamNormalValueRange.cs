using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査正常値範囲
/// </summary>
public class ExamNormalValueRange
{
    /// <summary>
    /// エラーレベル
    /// </summary>
    [JsonPropertyName("errorLevel")]
    public required int ErrorLevel { get; init; }

    /// <summary>
    /// 最大値
    /// </summary>
    [JsonPropertyName("maxValue")]
    public required decimal MaxValue { get; init; }

    /// <summary>
    /// 最小値
    /// </summary>
    [JsonPropertyName("minValue")]
    public required decimal MinValue { get; init; }

}
