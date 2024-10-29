using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査機器
/// </summary>
public class Equipment
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Equipment(int equipmentId, string equipmentName, int examMenuId)
    {
        EquipmentId = equipmentId;
        EquipmentName = equipmentName;
        ExamMenuId = examMenuId;
    }

    /// <summary>
    /// 検査機器ID
    /// </summary>
    [JsonPropertyName("equipmentId")]
    public int EquipmentId { get; }

    /// <summary>
    /// 検査機器名
    /// </summary>
    [JsonPropertyName("equipmentName")]
    public string EquipmentName { get; }

    /// <summary>
    /// 検査メニューID
    /// </summary>
    [JsonPropertyName("examMenuId")]
    public int ExamMenuId { get; }

}
