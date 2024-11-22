using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査機器
/// </summary>
public class Equipment
{
    /// <summary>
    /// 検査機器ID
    /// </summary>
    [JsonPropertyName("equipmentId")]
    public required int EquipmentId { get; init; }

    /// <summary>
    /// 検査機器名
    /// </summary>
    [JsonPropertyName("equipmentName")]
    public required string EquipmentName { get; init; }

    /// <summary>
    /// 検査メニューID
    /// </summary>
    [JsonPropertyName("examMenuId")]
    public required int ExamMenuId { get; init; }

    /// <summary>
    /// アプリ起動URL
    /// </summary>
    [JsonPropertyName("appLaunchUrl")]
    public required int AppLaunchUrl { get; init; }

    /// <summary>
    /// 処理スクリプトURL
    /// </summary>
    [JsonPropertyName("processingScriptUrl")]
    public required string ProcessingScriptUrl { get; init; }
}
