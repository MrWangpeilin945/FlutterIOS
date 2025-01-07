namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities
{
    /// <summary>
    /// 基準パターン
    /// </summary>
    public class ThresholdEntity
    {
        /// <summary>
        /// 基準パターンコード
        /// </summary>
        public required string ThresholdCode { get; init; }

        /// <summary>
        /// 基準パターン名
        /// </summary>
        public required string Name { get; init; }
    }
}
