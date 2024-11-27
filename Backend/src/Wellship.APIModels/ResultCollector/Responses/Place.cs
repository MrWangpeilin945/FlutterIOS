using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 会場
/// </summary>
public class Place
{
    /// <summary>
    /// 会場ID
    /// </summary>
    [JsonPropertyName("placeId")]
    public required int PlaceId { get; init; }

    /// <summary>
    /// 会場名
    /// </summary>
    [JsonPropertyName("placeName")]
    public required string PlaceName { get; init; }
}
