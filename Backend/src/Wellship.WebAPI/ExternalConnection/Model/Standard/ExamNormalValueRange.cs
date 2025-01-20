using System.Numerics;

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
        public required string Name { get; init; }
        /// <summary>
        /// 基準値パターンコード
        /// </summary>
        public required string ThresholdCd { get; init; }
        /// <summary>
        /// 検査項目明細コード
        /// </summary>
        public required string ExamItemDetailCd { get; init; }
        /// <summary>
        /// 対象年齢上限
        /// </summary>
        public required string MaxAge { get; init; }
        /// <summary>
        /// 対象年齢下限
        /// </summary>
        public required string MinAge { get; init; }
        /// <summary>
        /// 対象性別
        /// </summary>
        public required TargetSexType TargetSex { get; init; }
        /// <summary>
        /// 値上限
        /// </summary>
        public required decimal MaxValue { get; init; }
        /// <summary>
        /// 値下限
        /// </summary>
        public required decimal MinValue { get; init; }
        /// <summary>
        /// エラーレベル
        /// </summary>
        public required InputErrorLevel ErrorLevel { get; init; }
        /// <summary>
        /// エラーオブジェクトに登録する入力項目Noなど
        /// </summary>
        public required string InputNote { get; init; }
    }
}
