using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 中止理由
/// </summary>
public class CancelReason
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public CancelReason(int cancelReasonId, string cancelReasonName, int examItemId)
    {
        CancelReasonId = cancelReasonId;
        CancelReasonName = cancelReasonName;
        ExamItemId = examItemId;
    }

    /// <summary>
    /// 中止理由ID
    /// </summary>
    [JsonPropertyName("cancelReasonId")]
    public int CancelReasonId { get; }

    /// <summary>
    /// 中止理由名
    /// </summary>
    [JsonPropertyName("cancelReasonName")]
    public string CancelReasonName { get; }

    /// <summary>
    /// 検査項目ID
    /// </summary>
    [JsonPropertyName("examItemId")]
    public int ExamItemId { get; }

}
