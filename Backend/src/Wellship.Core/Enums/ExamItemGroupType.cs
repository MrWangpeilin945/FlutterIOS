namespace Ryobi.Wellship.Core.Enums;

/// <summary>
/// 検査項目グループ種別
/// コンポーネントを出し分けるためのタイプです。
/// </summary>
public enum ExamItemGroupType
{
    /// <summary>
    /// 自由入力
    /// </summary>
    自由入力 = 11,

    /// <summary>
    /// 数値
    /// </summary>
    数値 = 21,

    /// <summary>
    /// 数値_左右
    /// </summary>
    数値_左右 = 22,

    /// <summary>
    /// 選択
    /// </summary>
    選択 = 31,

    /// <summary>
    /// 選択_左右
    /// </summary>
    選択_左右 = 32,

    /// <summary>
    /// 身体計測
    /// </summary>
    身体計測 = 41,

    /// <summary>
    /// 血圧2回
    /// </summary>
    血圧2回 = 51,

    /// <summary>
    /// 視力
    /// </summary>
    視力 = 61,

    /// <summary>
    /// 聴力
    /// </summary>
    聴力 = 71,

    /// <summary>
    /// 通過
    /// </summary>
    通過 = 81
}
