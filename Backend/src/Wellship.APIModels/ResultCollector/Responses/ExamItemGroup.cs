using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査項目グループ
/// </summary>
public class ExamItemGroup
{
    /// <summary>
    /// 検査項目グループ種別
    /// </summary>
    [JsonPropertyName("type")]
    public required int Type { get; init;}

    /// <summary>
    /// 検査結果入力項目
    /// </summary>
    [JsonPropertyName("examItems")]
    public required InputExamItem[] ExamItems { get; init;}
}
