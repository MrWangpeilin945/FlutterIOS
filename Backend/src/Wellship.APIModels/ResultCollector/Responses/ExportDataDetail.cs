using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 連携対象検査結果明細
/// </summary>
public class ExportDataDetail
{
    /// <summary>
    /// 状態値
    /// </summary>
    [JsonPropertyName("status")]
    public required int Status { get; init; }

    /// <summary>
    /// 状態名
    /// </summary>
    [JsonPropertyName("statusName")]
    public required string StatusName { get; init; }

    /// <summary>
    /// 項目数
    /// </summary>
    [JsonPropertyName("count")]
    public required int Count { get; init; }

}
