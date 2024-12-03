using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査項目明細
/// </summary>
public class ExamItemDetail
{
    /// <summary>
    /// 配置番号
    /// </summary>
    [JsonPropertyName("positionNumber")]
    public required int PositionNumber { get; init; }

    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    [JsonPropertyName("examItemDetailId")]
    public required int ExamItemDetailId { get; init; }

    /// <summary>
    /// 機器ラベル
    /// </summary>
    [JsonPropertyName("equipmentLabel")]
    public required string EquipmentLabel { get; init; }

    /// <summary>
    /// 検査項目明細名
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// 検査実施するか
    /// </summary>
    [JsonPropertyName("hasOrder")]
    public required bool HasOrder { get; init; }

    /// <summary>
    /// 中止理由ID
    /// </summary>
    [JsonPropertyName("cancelReasonId")]
    public required int CancelReasonId { get; init; }

    /// <summary>
    /// 今回値
    /// </summary>
    [JsonPropertyName("value")]
    public required string Value { get; init; }

    /// <summary>
    /// 前回値
    /// </summary>
    [JsonPropertyName("prevValue")]
    public required string PrevValue { get; init; }

    /// <summary>
    /// 単位
    /// </summary>
    [JsonPropertyName("unit")]
    public required string Unit { get; init; }

    /// <summary>
    /// 検査項目明細種別
    /// </summary>
    [JsonPropertyName("type")]
    public required int Type { get; init; }

    /// <summary>
    /// 整数部最大桁数
    /// </summary>
    [JsonPropertyName("integerLength")]
    public required int IntegerLength { get; init; }

    /// <summary>
    /// 小数部有効桁数
    /// </summary>
    [JsonPropertyName("decimalLength")]
    public required int DecimalLength { get; init; }

    /// <summary>
    /// キーボード
    /// </summary>
    [JsonPropertyName("keyboard")]
    public required Keyboard Keyboard { get; init; }

    /// <summary>
    /// 選択肢
    /// </summary>
    [JsonPropertyName("examItemDetailOptions")]
    public required ExamItemDetailOption[] ExamItemDetailOptions { get; init; }

    /// <summary>
    /// 検査正常値範囲
    /// </summary>
    [JsonPropertyName("examNormalValueRanges")]
    public required ExamNormalValueRange[] ExamNormalValueRanges { get; init; }

}
