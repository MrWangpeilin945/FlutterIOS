using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 未受診の検査項目リスト
/// </summary>
public class UnexaminedMenuList
{
    /// <summary>
    /// 受診ID
    /// </summary>
    [JsonPropertyName("consultId")]
    public required int ConsultId { get; init; }

    /// <summary>
    /// 受診者ID
    /// </summary>
    [JsonPropertyName("examineeId")]
    public required int ExamineeId { get; init; }

    /// <summary>
    /// 受診者名
    /// </summary>
    [JsonPropertyName("examineeName")]
    public required string ExamineeName { get; init; }

    /// <summary>
    /// 未受診の検査メニューリスト
    /// </summary>
    [JsonPropertyName("unexaminedMenus")]
    public required ExamMenu[] UnexaminedMenus { get; init; }
}
