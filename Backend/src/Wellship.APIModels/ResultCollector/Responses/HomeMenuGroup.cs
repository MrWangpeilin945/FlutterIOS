using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// ホームメニューグループ
/// </summary>
public class HomeMenuGroup
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public HomeMenuGroup(string groupName, HomeMenu[] menus)
    {
        GroupName = groupName;
        Menus = menus;
    }

    /// <summary>
    /// ホームメニューグループ名
    /// </summary>
    [JsonPropertyName("groupName")]
    public string GroupName { get; }

    /// <summary>
    /// ホームメニュー
    /// </summary>
    [JsonPropertyName("menus")]
    public HomeMenu[] Menus { get; }
}
