using System.Text.Json.Serialization;
using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 基準値範囲
    /// </summary>
    public class ExamNormalValueRange
    {
        /// <summary>
        /// 名称
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; init; }
        /// <summary>
        /// 基準値パターンコード
        /// </summary>
        [JsonPropertyName("thresholdCode")]
        public required string ThresholdCode { get; init; }
        /// <summary>
        /// 検査項目明細コード
        /// </summary>
        [JsonPropertyName("examItemDetailCode")]
        public required string ExamItemDetailCode { get; init; }
        /// <summary>
        /// 対象年齢上限
        /// </summary>
        [JsonPropertyName("maxAge")]
        public required string MaxAge { get; init; }
        /// <summary>
        /// 対象年齢下限
        /// </summary>
        [JsonPropertyName("minAge")]
        public required string MinAge { get; init; }
        /// <summary>
        /// 対象性別
        /// </summary>
        [JsonPropertyName("targetSex")]
        public required TargetSexType TargetSex { get; init; }
        /// <summary>
        /// 値上限
        /// </summary>
        [JsonPropertyName("maxValue")]
        public required decimal MaxValue { get; init; }
        /// <summary>
        /// 値下限
        /// </summary>
        [JsonPropertyName("minValue")]
        public required decimal MinValue { get; init; }
        /// <summary>
        /// エラーレベル
        /// </summary>
        [JsonPropertyName("errorLevel")]
        public required InputErrorLevel ErrorLevel { get; init; }
        /// <summary>
        /// エラーオブジェクトに登録する入力項目Noなど
        /// </summary>
        [JsonPropertyName("inputNote")]
        public required string InputNote { get; init; }
    }
}
