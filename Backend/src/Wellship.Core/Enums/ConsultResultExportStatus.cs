namespace Ryobi.Wellship.Core.Enums;

/// <summary>
/// 受診データ出力状況
/// 受診単位のデータ出力ステータスです。
/// </summary>
public enum ConsultResultExportStatus
{
    /// <summary>
    /// 未出力
    /// </summary>
    未出力 = 11,

    /// <summary>
    /// 出力保留
    /// </summary>
    出力保留 = 21,

    /// <summary>
    /// 出力済み
    /// </summary>
    出力済み = 31
}
