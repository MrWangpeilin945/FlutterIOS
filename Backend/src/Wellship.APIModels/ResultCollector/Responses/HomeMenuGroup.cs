using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// ホームメニューグループ
/// </summary>
public class HomeMenuGroup
{
    /// <summary>
    /// ホームメニューグループ名
    /// </summary>
    [JsonPropertyName("groupName")]
    public required string GroupName { get; init; }

    /// <summary>
    /// ホームメニュー
    /// </summary>
    [JsonPropertyName("menus")]
    public required HomeMenu[] Menus { get; init; }
}
