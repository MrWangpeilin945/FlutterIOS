using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 班ごとの会場日程リスト
/// </summary>
public class PlaceSchedulePlaces
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PlaceSchedulePlaces(int teamId, string teamName, DateOnly examDate, ICollection<PlaceSchedule> placeSchedules)
    {
        TeamId = teamId;
        TeamName = teamName;
        ExamDate = examDate;
        PlaceSchedules = placeSchedules.ToArray();
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
    /// 健診日
    /// </summary>
    [JsonPropertyName("examDate")]
    public DateOnly ExamDate { get; }

    /// <summary>
    /// 会場日程リスト
    /// </summary>
    [JsonPropertyName("placeSchedules")]
    public PlaceSchedule[] PlaceSchedules { get; }

}
