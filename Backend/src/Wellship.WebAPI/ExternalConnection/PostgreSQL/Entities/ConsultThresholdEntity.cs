namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 基準値エンティティ
    /// </summary>
    public class ConsultThresholdEntity
    {
        /// <summary>
        /// 基準値パターンID
        /// </summary>
        public required Guid ThresholdId { get; init; }

        /// <summary>
        /// 優先度
        /// </summary>
        public required int Priority { get; init; }
    }
}
