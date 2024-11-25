using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 連携対象検査結果リスト
/// </summary>
public class ExportDataList
{
    /// <summary>
    /// 連携対象検査結果リスト
    /// </summary>
    [JsonPropertyName("exportData")]
    public required ExportData[] ExportData { get; init; }

}
