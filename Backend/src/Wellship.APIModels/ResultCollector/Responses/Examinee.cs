using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 受診者情報
/// </summary>
public class Examinee
{
    /// <summary>
    /// 受付番号
    /// </summary>
    [JsonPropertyName("ticketNumber")]
    public required string TicketNumber { get; init; }

    /// <summary>
    /// 氏名
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; } = "";

    /// <summary>
    /// カナ氏名
    /// </summary>
    [JsonPropertyName("kanaName")]
    public required string KanaName { get; init; } = "";

    /// <summary>
    /// 生年月日
    /// </summary>
    [JsonPropertyName("birthdate")]
    public required DateOnly Birthdate { get; init; }    

    /// <summary>
    /// 性別
    /// </summary>
    [JsonPropertyName("sex")]
    public required int Sex { get; init; }    

    /// <summary>
    /// 事業所名
    /// </summary>
    [JsonPropertyName("organizations")]
    public required string[] Organizations { get; init; }    

    /// <summary>
    /// 同姓同名アラート
    /// </summary>
    [JsonPropertyName("sameNameAlert")]
    public required bool SameNameAlert { get; init; }    

    /// <summary>
    /// 受診日年齢
    /// </summary>
    [JsonPropertyName("examDateAge")]
    public required int ExamDateAge { get; init; }    

}
