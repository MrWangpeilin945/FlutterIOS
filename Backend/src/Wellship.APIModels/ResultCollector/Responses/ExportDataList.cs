using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 連携対象検査結果リスト
/// </summary>
public class ExportDataList
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ExportDataList(ExportData[] exportData)
    {
        ExportData = exportData;
    }

    /// <summary>
    /// 連携対象検査結果リスト
    /// </summary>
    [JsonPropertyName("exportData")]
    public ExportData[] ExportData { get; }

}
