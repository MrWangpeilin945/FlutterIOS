using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 中止理由リスト
/// </summary>
public class CancelReasonList
{
    /// <summary>
    /// 中止理由リスト
    /// </summary>
    [JsonPropertyName("cancelReasons")]
    public required CancelReason[] CancelReasons { get; init; }
}
