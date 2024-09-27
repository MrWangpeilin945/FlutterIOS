using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 会場日程の班
/// </summary>
public class PlaceScheduleTeam
{

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PlaceScheduleTeam(int teamId, string teamName, ICollection<Place> places)
    {
        TeamId = teamId;
        TeamName = teamName;
        Places = places.ToArray();
    }

    /// <summary>
    /// 班ID
    /// </summary>
    [JsonPropertyName("teamId")]
    public int TeamId { get; }

    /// <summary>
    /// 班名
    /// </summary>
    [JsonPropertyName("teamName")]
    public string TeamName { get; }

    /// <summary>
    /// 会場リスト
    /// </summary>
    [JsonPropertyName("places")]
    public Place[] Places { get; }
}