using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// ホームメニュー
/// </summary>
public class HomeMenu
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public HomeMenu(string menuName, string path, string[] availableConditions)
    {
        MenuName = menuName;
        Path = path;
        AvailableConditions = availableConditions;
    }

    /// <summary>
    /// ホームメニュー名
    /// </summary>
    [JsonPropertyName("menuName")]
    public string MenuName { get; }

    /// <summary>
    /// 遷移パス
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; }

    /// <summary>
    /// 使用可能条件
    /// </summary>
    [JsonPropertyName("availableConditions")]
    public string[] AvailableConditions { get; }

}
