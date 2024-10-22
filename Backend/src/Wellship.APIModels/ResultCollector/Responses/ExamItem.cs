using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査項目
/// </summary>
public class ExamItem
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ExamItem(int examItemId, string examItemName)
    {
        ExamItemId = examItemId;
        ExamItemName = examItemName;
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
}
