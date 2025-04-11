using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 受診者情報
/// </summary>
public class ConsultExaminee
{
    /// <summary>
    /// 受診番号
    /// </summary>
    [JsonPropertyName("consultNumber")]
    public required string ConsultNumber { get; init; }

    /// <summary>
    /// 受付番号
    /// </summary>
    [JsonPropertyName("ticketNumber")]
    public required string TicketNumber { get; init; }

    /// <summary>
    /// カナ氏名
    /// </summary>
    [JsonPropertyName("kanaName")]
    public required string KanaName { get; init; }

    /// <summary>
    /// 性別
    /// </summary>
    [JsonPropertyName("sex")]
    public required int Sex { get; init; }

    /// <summary>
    /// 受付日時
    /// </summary>
    [JsonPropertyName("checkedInAt")]
    public DateTimeOffset? CheckedInAt { get; init; }
}