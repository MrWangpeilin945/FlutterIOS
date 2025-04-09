using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 受診者情報リスト
/// </summary>
public class ConsultExamineeList
{
    /// <summary>
    /// 受診者情報リスト
    /// </summary>
    [JsonPropertyName("examinees")]
    public required ConsultExaminee[] Examinees { get; init; }
}