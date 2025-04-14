namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 集計検査進捗の明細
/// 進捗画面で取り扱う進捗ステータス。
/// 受診の進捗状況と検査項目明細に対する結果あるいは中止レコードの有無から算出する。
/// </summary>
public class AggregatedProgressDetail
{
    /// <summary>
    /// 検査メニューID
    /// </summary>
    public required int ExamMenuId { get; init; }

    /// <summary>
    /// 検査メニュー名
    /// </summary>
    public required string ExamMenuName { get; init; }

    /// <summary>
    /// 予定の件数
    /// </summary>
    public required int Count11 { get; init; }

    /// <summary>
    /// 来場の件数
    /// </summary>
    public required int Count21 { get; init; }

    /// <summary>
    /// 済の件数
    /// </summary>
    public required int Count41 { get; init; }

    /// <summary>
    /// 中止の件数
    /// </summary>
    public required int Count51 { get; init; }

    /// <summary>
    /// その他の件数
    /// </summary>
    public required int Count71 { get; init; }
}
