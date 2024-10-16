using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Requests;

/// <summary>
/// 受診番号検証用リクエストモデル
/// </summary>
public class ConsultNumberRequest
{
    /// <summary>
    /// 受診番号
    /// </summary>
    [JsonPropertyName("consultNumber")]
    public string ConsultNumber { get; set; } = "";
}
