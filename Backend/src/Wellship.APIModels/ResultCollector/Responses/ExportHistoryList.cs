using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査結果出力履歴リスト
/// </summary>
public class ExportHistoryList
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ExportHistoryList(ExportHistory[] exportHistories)
    {
        ExportHistories = exportHistories;
    }
    /// <summary>
    /// 検査結果出力履歴リスト
    /// </summary>
    [JsonPropertyName("exportHistories")]
    public ExportHistory[] ExportHistories { get; }

}
