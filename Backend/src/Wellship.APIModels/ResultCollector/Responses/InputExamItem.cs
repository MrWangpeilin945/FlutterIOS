using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査結果入力項目
/// </summary>
public class InputExamItem
{
    /// <summary>
    /// 配置番号
    /// </summary>
    [JsonPropertyName("positionNumber")]
    public required int PositionNumber { get; init;}

    /// <summary>
    /// 検査項目ID
    /// </summary>
    [JsonPropertyName("examItemId")]
    public required int ExamItemId { get; init;}

    /// <summary>
    /// 検査項目名
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init;}

    /// <summary>
    /// 検査項目明細
    /// </summary>
    [JsonPropertyName("examItemDetails")]
    public required ExamItemDetail[] ExamItemDetails { get; init;}

    /// <summary>
    /// 検査結果登録エラー
    /// </summary>
    [JsonPropertyName("examRegstResults")]
    public required ExamRegstResult[] ExamRegstResults { get; init;}

}