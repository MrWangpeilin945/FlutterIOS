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

}
