namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities
{
    /// <summary>
    /// 基準値（範囲）エンティティ
    /// </summary>
    public class ExamNormalValueRangeEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        public required string Name { get; init; }
        /// <summary>
        /// 基準値パターンID
        /// </summary>
        public required Guid ThresholdId { get; init; }
        /// <summary>
        /// 検査項目明細ID
        /// </summary>
        public required int ExamItemDetailId { get; init; }
        /// <summary>
        /// 対象年齢下限
        /// </summary>
        public required string MinAge { get; init; }
        /// <summary>
        /// 対象年齢上限
        /// </summary>
        public required string MaxAge { get; init; }
        /// <summary>
        /// 対象性別
        /// </summary>
        public required int TargetSex { get; init; }
        /// <summary>
        /// 値下限
        /// </summary>
        public required decimal MinValue { get; init; }
        /// <summary>
        /// 値上限
        /// </summary>
        public required decimal MaxValue { get; init; }
        /// <summary>
        /// エラーレベル
        /// </summary>
        public required int ErrorLevel { get; init; }
    }
}
