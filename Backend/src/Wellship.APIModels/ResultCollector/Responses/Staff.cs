using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 職員
/// </summary>
public class Staff
{
    /// <summary>
    /// 職員ID
    /// </summary>
    [JsonPropertyName("staffId")]
    public required Guid StaffId { get; init; }

    /// <summary>
    /// 職員名
    /// </summary>
    [JsonPropertyName("staffName")]
    public required string StaffName { get; init; }
}
