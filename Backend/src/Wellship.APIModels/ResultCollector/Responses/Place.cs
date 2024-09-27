using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 会場
/// </summary>
public class Place
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Place(int placeId, string placeName)
    {
        PlaceId = placeId;
        PlaceName = placeName;
    }

    /// <summary>
    /// 会場ID
    /// </summary>
    [JsonPropertyName("placeId")]
    public int PlaceId { get; }

    /// <summary>
    /// 会場名
    /// </summary>
    [JsonPropertyName("placeName")]
    public string PlaceName { get; }
}
