using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査項目明細と子要素
/// キーボードと選択肢
/// </summary>
public class ExamItemDetailChild
{
    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public required int ExamItemDetailId { get; init; }

    /// <summary>
    /// 検査項目明細名
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 配置番号
    /// </summary>
    public required int PositionNumber { get; init; }

    /// <summary>
    /// 機器ラベル
    /// </summary>
    public required string EquipmentLabel { get; init; }

    /// <summary>
    /// 単位
    /// </summary>
    public required string Unit { get; init; }

    /// <summary>
    /// 検査項目明細種別
    /// </summary>
    public required ExamItemDetailType Type { get; init; }

    /// <summary>
    /// 整数部最大桁数
    /// </summary>
    public required int IntegerLength { get; init; }

    /// <summary>
    /// 小数部有効桁数
    /// </summary>
    public required int DecimalLength { get; init; }

    /// <summary>
    /// キーボード種別
    /// </summary>
    public required KeyboardType KeyboardType { get; init; }

    /// <summary>
    /// キーボード入力値リスト
    /// </summary>
    public required IEnumerable<Keyboard> Keyboards { get; init; }

    /// <summary>
    /// 選択肢
    /// </summary>
    public required IEnumerable<ExamItemDetailOption> DetailOptions { get; init; }
}
