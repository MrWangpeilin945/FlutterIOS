using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査結果入力画面 受診者情報
/// </summary>
public class InputExamExaminee
{
    /// <summary>
    /// 受付番号
    /// </summary>
    [JsonPropertyName("ticketNumber")]
    public required string TicketNumber { get; init;}

    /// <summary>
    /// カナ氏名
    /// </summary>
    [JsonPropertyName("kanaName")]
    public required string KanaName { get; init;}

    /// <summary>
    /// 性別
    /// </summary>
    [JsonPropertyName("sex")]
    public required int Sex { get; init;}    

    /// <summary>
    /// 受診日年齢
    /// </summary>
    [JsonPropertyName("examDateAge")]
    public required int ExamDateAge { get; init;}    

}
