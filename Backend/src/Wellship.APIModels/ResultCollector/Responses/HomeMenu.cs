using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// ホームメニュー
/// </summary>
public class HomeMenu
{
    /// <summary>
    /// ホームメニュー名
    /// </summary>
    [JsonPropertyName("menuName")]
    public required string MenuName { get; init; }

    /// <summary>
    /// 遷移パス
    /// </summary>
    [JsonPropertyName("path")]
    public required string Path { get; init; }

    /// <summary>
    /// 使用可能条件
    /// </summary>
    [JsonPropertyName("availableConditions")]
    public required string[] AvailableConditions { get; init; }

}
