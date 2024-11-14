namespace Ryobi.Wellship.Core.Enums;

/// <summary>
/// 受診進捗状況
/// 受診単位の進捗ステータスです。
/// </summary>
public enum ConsultProgressStatus
{
    /// <summary>
    /// 来場待ち
    /// </summary>
    来場待ち = 11,

    /// <summary>
    /// 検査中
    /// </summary>
    検査中 = 21,

    /// <summary>
    /// キャンセル
    /// </summary>
    キャンセル = 41
}