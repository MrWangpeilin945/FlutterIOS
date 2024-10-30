using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 中止理由リスト
/// </summary>
public class CancelReasonList
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public CancelReasonList(CancelReason[] cancelReasons)
    {
        CancelReasons = cancelReasons;
    }

    /// <summary>
    /// 中止理由リスト
    /// </summary>
    [JsonPropertyName("cancelReasons")]
    public CancelReason[] CancelReasons { get; }

}
