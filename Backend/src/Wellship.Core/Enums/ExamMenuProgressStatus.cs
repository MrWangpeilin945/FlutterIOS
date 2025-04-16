namespace Ryobi.Wellship.Core.Enums;

/// <summary>
/// 検査進捗状況
/// 検査メニュー単位の進捗ステータスです。
/// </summary>
public enum ExamProgressStatus
{
    /// <summary>
    /// 依頼なし
    /// </summary>
    依頼なし = 10,

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