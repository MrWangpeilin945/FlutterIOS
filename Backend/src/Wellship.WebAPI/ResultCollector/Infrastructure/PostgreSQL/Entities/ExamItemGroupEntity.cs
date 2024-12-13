using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 検査項目設定情報
/// </summary>
public class ExamItemGroupEntity
{
    /// <summary>
    /// 検査項目グループID
    /// </summary>
    public required int ExamItemGroupId { get; init; }

    /// <summary>
    /// 検査項目グループ種別
    /// </summary>
    public required ExamItemGroupType GroupType { get; init; }

    /// <summary>
    /// 検査項目配置番号
    /// </summary>
    public required int ExamItemPositionNumber { get; init; }

    /// <summary>
    /// 検査項目ID
    /// </summary>
    public required int ExamItemId { get; init; }

    /// <summary>
    /// 検査項目名
    /// </summary>
    public required string ExamItemName { get; init; }

    /// <summary>
    /// 検査項目明細配置番号
    /// </summary>
    public required int ExamItemDetailPositionNumber { get; init; }

    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public required int ExamItemDetailId { get; init; }

    /// <summary>
    /// 機器ラベル
    /// </summary>
    public required string EquipmentLabel { get; init; }

    /// <summary>
    /// 検査項目明細名
    /// </summary>
    public required string ExamItemDetailName { get; init; }

    /// <summary>
    /// 単位
    /// </summary>
    public required string Unit { get; init; }

    /// <summary>
    /// 検査項目明細種別
    /// </summary>
    public required ExamItemDetailType ExamItemDetailType { get; init; }

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
    /// 選択肢表示順
    /// </summary>
    public required int OptionOrderNumber { get; init; }

    /// <summary>
    /// 選択肢コード
    /// </summary>
    public required string OptionCode { get; init; }

    /// <summary>
    /// 選択肢名称
    /// </summary>
    public required string OptionName { get; init; }

    /// <summary>
    /// 検査正常値範囲-エラーレベル
    /// </summary>
    public required InputErrorLevel ErrorLevel { get; init; }

    /// <summary>
    /// 検査正常値範囲-最大値
    /// </summary>
    public required int MaxValue { get; init; }

    /// <summary>
    /// 検査正常値範囲-最小値
    /// </summary>
    public required int MinValue { get; init; }

}
