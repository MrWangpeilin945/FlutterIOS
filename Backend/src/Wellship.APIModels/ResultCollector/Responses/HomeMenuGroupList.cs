using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// ホームメニューグループリスト
/// </summary>
public class HomeMenuGroupList
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public HomeMenuGroupList(HomeMenuGroup[] homeMenuGroups)
    {
        HomeMenuGroups = homeMenuGroups;
    }

    /// <summary>
    /// ホームメニューグループリスト
    /// </summary>
    [JsonPropertyName("homeMenuGroups")]
    public HomeMenuGroup[] HomeMenuGroups { get; }

}
