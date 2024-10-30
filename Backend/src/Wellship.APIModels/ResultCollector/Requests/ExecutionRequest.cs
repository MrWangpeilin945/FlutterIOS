using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Requests;

/// <summary>
/// 実施有無と中止理由のリクエストモデル
/// </summary>
public class ExecutionRequest
{
    /// <summary>
    /// 検査項目ID
    /// </summary>
    [JsonPropertyName("examItemId")]
    public int ExamItemId { get; set; }

    /// <summary>
    /// 検査実施するか
    /// </summary>
    [JsonPropertyName("isPerforming")]
    public bool IsPerforming { get; set; }

    /// <summary>
    /// 中止理由ID
    /// </summary>
    [JsonPropertyName("cancelReasonId")]
    public int? CancelReasonId { get; set; }
}
