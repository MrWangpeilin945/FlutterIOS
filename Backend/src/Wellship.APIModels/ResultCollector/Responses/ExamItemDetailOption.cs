using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査項目明細選択肢
/// </summary>
public class ExamItemDetailOption
{
    /// <summary>
    /// 表示順
    /// </summary>
    [JsonPropertyName("orderNumber")]
    public required int OrderNumber { get; init; }

    /// <summary>
    /// 選択肢コード
    /// </summary>
    [JsonPropertyName("code")]
    public required string Code { get; init; }

    /// <summary>
    /// 選択肢名称
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

}
