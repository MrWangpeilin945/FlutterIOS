using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Requests;

/// <summary>
/// 検査結果の登録のリクエストモデル
/// </summary>
public class ResultsRequest
{
    /// <summary>
    /// 検査メニューID
    /// </summary>
    [JsonPropertyName("examMenuId")]
    public required int ExamMenuId { get; set; }

    /// <summary>
    /// 検査項目
    /// </summary>
    [JsonPropertyName("examResults")]
    public required ResultRequest[] ExamResults { get; set; }

}