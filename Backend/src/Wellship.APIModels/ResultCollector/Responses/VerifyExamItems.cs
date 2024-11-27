using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査結果検証結果情報
/// </summary>
public class VerifyExamItems
{
    /// <summary>
    /// エラーレベル
    /// </summary>
    [JsonPropertyName("errorLevel")]
    public required int ErrorLevel { get; init; }

    /// <summary>
    /// 検査結果入力項目グループ
    /// </summary>
    [JsonPropertyName("examItemGroups")]
    public required ExamItemGroup[] ExamItemGroups { get; init; }

}
