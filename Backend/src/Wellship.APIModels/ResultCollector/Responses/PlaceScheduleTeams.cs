using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 会場日程の班リスト
/// </summary>
public class PlaceScheduleTeams
{
    /// <summary>
    /// 班リスト
    /// </summary>
    [JsonPropertyName("teams")]
    public required PlaceScheduleTeam[] Teams { get; init; }
}
