using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 中止理由
/// </summary>
public class CancelReason
{
    /// <summary>
    /// 中止理由ID
    /// </summary>
    [JsonPropertyName("cancelReasonId")]
    public required int CancelReasonId { get; init; }

    /// <summary>
    /// 中止理由名
    /// </summary>
    [JsonPropertyName("cancelReasonName")]
    public required string CancelReasonName { get; init; }

    /// <summary>
    /// 検査項目ID
    /// </summary>
    [JsonPropertyName("examItemId")]
    public required int ExamItemId { get; init; }

}
