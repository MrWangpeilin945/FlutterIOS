using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 関連検査項目
/// </summary>
public class RelatedExamItem
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public RelatedExamItem(string examItemName, string examResult)
    {
        ExamItemName = examItemName;
        ExamResult = examResult;
    }

    /// <summary>
    /// 検査項目名
    /// </summary>
    [JsonPropertyName("examItemName")]
    public string ExamItemName { get; }

    /// <summary>
    /// 検査結果
    /// </summary>
    [JsonPropertyName("examResult")]
    public string ExamResult { get; }

}
