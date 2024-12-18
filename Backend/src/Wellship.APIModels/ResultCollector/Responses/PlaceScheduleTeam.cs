using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 会場日程の班
/// </summary>
public class PlaceScheduleTeam
{
    /// <summary>
    /// 班ID
    /// </summary>
    [JsonPropertyName("teamId")]
    public required Guid TeamId { get; init; }

    /// <summary>
    /// 班名
    /// </summary>
    [JsonPropertyName("teamName")]
    public required string TeamName { get; init; }

    /// <summary>
    /// 会場リスト
    /// </summary>
    [JsonPropertyName("places")]
    public required Place[] Places { get; init; }
}