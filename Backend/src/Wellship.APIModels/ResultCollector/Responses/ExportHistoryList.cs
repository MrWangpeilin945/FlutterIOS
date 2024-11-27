using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査結果出力履歴リスト
/// </summary>
public class ExportHistoryList
{
    /// <summary>
    /// 検査結果出力履歴リスト
    /// </summary>
    [JsonPropertyName("exportHistories")]
    public required ExportHistory[] ExportHistories { get; init; }
}
