using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 進捗状況
/// </summary>
public class Progress
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Progress(int examItemId, string examItemName, ProgressDetail[] details)
    {
        ExamItemId = examItemId;
        ExamItemName = examItemName;
        Details = details;
    }

    /// <summary>
    /// 検査項目ID
    /// </summary>
    [JsonPropertyName("examItemId")]
    public int ExamItemId { get; }

    /// <summary>
    /// 検査項目名
    /// </summary>
    [JsonPropertyName("examItemName")]
    public string ExamItemName { get; }

    /// <summary>
    /// 進捗明細
    /// </summary>
    [JsonPropertyName("details")]
    public ProgressDetail[] Details { get; }

}
