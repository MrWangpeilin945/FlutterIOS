using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// ホームメニューグループリスト
/// </summary>
public class HomeMenuGroupList
{
    /// <summary>
    /// ホームメニューグループリスト
    /// </summary>
    [JsonPropertyName("homeMenuGroups")]
    public required HomeMenuGroup[] HomeMenuGroups { get; init; }

    /// <summary>
    /// 会場日程状況
    /// </summary>
    [JsonPropertyName("placeScheduleLockingStatus")]
    public required int? PlaceScheduleLockingStatus { get; init; }
}
