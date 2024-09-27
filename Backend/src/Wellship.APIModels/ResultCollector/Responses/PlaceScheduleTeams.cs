using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 会場日程の班リスト
/// </summary>
public class PlaceScheduleTeams
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PlaceScheduleTeams(ICollection<PlaceScheduleTeam> placeScheduleTeam)
    {
        Teams = placeScheduleTeam.ToArray();
    }

    /// <summary>
    /// 班リスト
    /// </summary>
    [JsonPropertyName("teams")]
    public PlaceScheduleTeam[] Teams { get; }
}
