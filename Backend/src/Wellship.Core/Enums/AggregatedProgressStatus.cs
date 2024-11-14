namespace Ryobi.Wellship.Core.Enums;

/// <summary>
/// 集計検査進捗状況
/// 進捗画面で取り扱うステータスです。
/// </summary>
public enum AggregatedProgressStatus
{
    /// <summary>
    /// 予定
    /// </summary>
    予定 = 11,

    /// <summary>
    /// 来場
    /// </summary>
    来場 = 21,

    /// <summary>
    /// 済
    /// </summary>
    済 = 41,

    /// <summary>
    /// 中止
    /// </summary>
    中止 = 51
}
