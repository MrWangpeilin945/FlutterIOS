using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査機器リスト
/// </summary>
public class EquipmentList
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public EquipmentList(Equipment[] equipments)
    {
        Equipments = equipments;
    }

    /// <summary>
    /// 検査機器リスト
    /// </summary>
    [JsonPropertyName("equipments")]
    public Equipment[] Equipments { get; }

}
