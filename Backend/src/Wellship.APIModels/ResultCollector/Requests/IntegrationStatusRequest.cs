using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Requests;

/// <summary>
/// 検査結果出力状況を未出力に戻すリクエストモデル
/// </summary>
public class UndoIntegrationExportStatusRequest
{
    /// <summary>
    /// 出力履歴ID
    /// </summary>
    [JsonPropertyName("exportId")]
    public Guid ExportId { get; set; }
}
