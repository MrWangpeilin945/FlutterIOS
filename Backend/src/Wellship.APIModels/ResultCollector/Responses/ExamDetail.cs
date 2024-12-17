using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 実施検査状況
/// </summary>
public class ExamDetail
{
    /// <summary>
    /// 検査項目ID
    /// </summary>
    [JsonPropertyName("examItemId")]
    public required int ExamItemId { get; init;}

    /// <summary>
    /// 検査項目名
    /// </summary>
    [JsonPropertyName("examItemName")]
    public required string ExamItemName { get; init;}

    /// <summary>
    /// 検査依頼が存在するか
    /// </summary>
    [JsonPropertyName("hasOrder")]

    public required bool HasOrder { get; init;}

    /// <summary>
    /// 中止理由ID
    /// </summary>
    [JsonPropertyName("cancelReasonId")]

    public required int? CancelReasonId { get; init;}
}
