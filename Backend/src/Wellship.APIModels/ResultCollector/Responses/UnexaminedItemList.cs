using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 未受診の検査項目リスト
/// </summary>
public class UnexaminedItemList
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public UnexaminedItemList(int consultId, int examineeId, string examineeName, ICollection<ExamItem> examItems)
    {
        ConsultId = consultId;
        ExamineeId = examineeId;
        ExamineeName = examineeName;
        UnexaminedItems = examItems.ToArray();
    }

    /// <summary>
    /// 受診ID
    /// </summary>
    [JsonPropertyName("consultId")]
    public int ConsultId { get; }

    /// <summary>
    /// 受診者ID
    /// </summary>
    [JsonPropertyName("examineeId")]
    public int ExamineeId { get; }

    /// <summary>
    /// 受診者名
    /// </summary>
    [JsonPropertyName("examineeName")]
    public string ExamineeName { get; }

    /// <summary>
    /// 未受診の検査項目リスト
    /// </summary>
    [JsonPropertyName("unexaminedItems")]
    public ExamItem[] UnexaminedItems { get; }
}
