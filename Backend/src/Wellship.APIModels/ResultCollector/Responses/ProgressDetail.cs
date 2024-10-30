using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 進捗明細
/// </summary>
public class ProgressDetail
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ProgressDetail(int status, string statusName, int count)
    {
        Status = status;
        StatusName = statusName;
        Count = count;
    }

    /// <summary>
    /// 状態値
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; }

    /// <summary>
    /// 状態名
    /// </summary>
    [JsonPropertyName("statusName")]
    public string StatusName { get; }

    /// <summary>
    /// 対象者数
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; }

}
