using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 実施検査状況
/// </summary>
public class ExamDetail
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ExamDetail(int examItemId, string examItemName, bool isPerforming, int cancelReasonId)
    {
        ExamItemId = examItemId;
        ExamItemName = examItemName;
        IsPerforming = isPerforming;
        CancelReasonId = cancelReasonId;
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
    /// 検査実施するか
    /// </summary>
    [JsonPropertyName("isPerforming")]

    public bool IsPerforming { get; }

    /// <summary>
    /// 中止理由ID
    /// </summary>
    [JsonPropertyName("cancelReasonId")]

    public int CancelReasonId { get; }
}
