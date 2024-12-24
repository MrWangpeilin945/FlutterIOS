using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査項目明細マスタのエンティティ
/// </summary>
public class ExamItemDetailChildrenEntity
{
    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public required int ExamItemDetailId { get; set; }
   
    /// <summary>
    /// 検査項目明細名
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// 配置番号
    /// </summary>
    public required int PositionNumber { get; set; }

    /// <summary>
    /// 機器ラベル
    /// </summary>
    public required string EquipmentLabel { get; set; }

    /// <summary>
    /// 単位
    /// </summary>
    public required string Unit { get; set; }

    /// <summary>
    /// 検査項目明細種別
    /// </summary>
    public required ExamItemDetailType Type { get; set; }

    /// <summary>
    /// キーボード種別
    /// </summary>
    public required KeyboardType KeyboardType { get; set; }

    /// <summary>
    /// 整数部桁数
    /// </summary>
    public required int IntegerLength { get; set; }

    /// <summary>
    /// 小数部桁数
    /// </summary>
    public required int DecimalLength { get; set; }

    /// <summary>
    /// キーボードID
    /// </summary>
    public  required int KeyboardId {get; set; }

    /// <summary>
    /// キーボード入力値
    /// </summary>
    public required string KeyboardValue { get; set; }

    /// <summary>
    /// 選択肢ID
    /// </summary>
    public required string OptionId { get; set; }

    /// <summary>
    /// 選択肢コード
    /// </summary>
    public required string OptionCode { get; set; }

    /// <summary>
    /// 選択肢名称
    /// </summary>
    public required string OptionName { get; set; }

    /// <summary>
    /// 選択肢表示順
    /// </summary>
    public required int OrderNumber { get; set; }

}
