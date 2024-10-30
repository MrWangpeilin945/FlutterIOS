using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 連携対象検査結果明細
/// </summary>
public class ExportDataDetail
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ExportDataDetail(int status, string statusName, int count)
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
    /// 項目数
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; }

}
