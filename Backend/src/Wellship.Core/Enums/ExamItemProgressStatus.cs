namespace Ryobi.Wellship.Core.Enums;

/// <summary>
/// 検査進捗状況
/// 検査項目単位の進捗ステータスです。
/// </summary>
public enum ExamItemProgressStatus
{
    /// <summary>
    /// 未実施
    /// </summary>
    未実施 = 11,

    /// <summary>
    /// 検査済み
    /// </summary>
    検査済み = 41,

    /// <summary>
    /// 検査中止
    /// </summary>
    検査中止 = 51
}