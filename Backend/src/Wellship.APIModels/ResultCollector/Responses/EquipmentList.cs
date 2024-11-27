using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査機器リスト
/// </summary>
public class EquipmentList
{
    /// <summary>
    /// 検査機器リスト
    /// </summary>
    [JsonPropertyName("equipments")]
    public required Equipment[] Equipments { get; init; }

}
