using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 職員
/// </summary>
public class Staff
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Staff(int staffId, string staffName)
    {
        StaffId = staffId;
        StaffName = staffName;
    }

    /// <summary>
    /// 職員ID
    /// </summary>
    [JsonPropertyName("staffId")]
    public int StaffId { get; }

    /// <summary>
    /// 職員名
    /// </summary>
    [JsonPropertyName("staffName")]
    public string StaffName { get; }
}
