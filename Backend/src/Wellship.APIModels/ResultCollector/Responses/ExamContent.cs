using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査内容
/// </summary>
public class ExamContent
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ExamContent(string consultNumber, Examinee examinee, bool isComplete, RelatedExamItem[] relatedExamItems, 
                      ExamDetail[] examItems, string[] examDecisionResult, ExamMenu[] unexaminedItems)
    {
        ConsultNumber = consultNumber;
        Examinee = examinee;
        IsComplete = isComplete;
        RelatedExamItems = relatedExamItems;
        ExamItems = examItems;
        ExamDecisionResult = examDecisionResult;
        UnexaminedItems = unexaminedItems;
    }

    /// <summary>
    /// 受診番号
    /// </summary>
    [JsonPropertyName("consultNumber")]
    public string ConsultNumber { get; }

    /// <summary>
    /// 受診者情報
    /// </summary>
    [JsonPropertyName("examinee")]
    public Examinee Examinee { get; }

    /// <summary>
    /// 検査実施判断
    /// </summary>
    [JsonPropertyName("isComplete")]
    public bool IsComplete { get; }

    /// <summary>
    /// 関連検査項目
    /// </summary>
    [JsonPropertyName("relatedExamItems")]
    public RelatedExamItem[] RelatedExamItems { get; }

    /// <summary>
    /// 実施検査項目
    /// </summary>
    [JsonPropertyName("examItems")]
    public ExamDetail[] ExamItems { get; }

    /// <summary>
    /// 検査実施判断結果
    /// </summary>
    [JsonPropertyName("examDecisionResult")]
    public string[] ExamDecisionResult { get; }

    /// <summary>
    /// 未実施検査項目
    /// </summary>
    [JsonPropertyName("unexaminedItems")]
    public ExamMenu[] UnexaminedItems { get; }

}
