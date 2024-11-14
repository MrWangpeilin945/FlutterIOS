namespace Ryobi.Wellship.Core.Enums;

/// <summary>
/// 会場日程データ出力状況
/// 会場日程単位のデータ出力ステータスです。 
/// </summary>
public enum PlaceScheduleResultExportStatus
{
    /// <summary>
    /// 未出力
    /// </summary>
    未出力 = 11,

    /// <summary>
    /// 出力済み
    /// </summary>
    出力済み = 31,

    /// <summary>
    /// 出力エラー
    /// </summary>
    出力エラー = 41
}
