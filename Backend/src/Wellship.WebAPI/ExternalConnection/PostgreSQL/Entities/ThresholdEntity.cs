namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities
{
    /// <summary>
    /// 基準パターン
    /// </summary>
    public class ThresholdEntity
    {
        /// <summary>
        /// 基準値パターンID
        /// </summary>
        public Guid ThresholdId { get; init; }

        /// <summary>
        /// 基準値パターンコード
        /// </summary>
        public required string ThresholdCode { get; init; }

        /// <summary>
        /// 基準値パターン名
        /// </summary>
        public required string Name { get; init; }
    }
}
