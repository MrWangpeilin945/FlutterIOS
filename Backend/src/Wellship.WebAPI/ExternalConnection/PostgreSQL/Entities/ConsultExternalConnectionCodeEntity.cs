namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 受診外部連携キーエンティティ
    /// </summary>
    public class ConsultExternalConnectionCodeEntity
    {
        /// <summary>
        /// 受診ID
        /// </summary>
        public required Guid ConsultId { get; init; }

        /// <summary>
        /// 外部連携キー
        /// </summary>
        public required string ExternalConnectionCode { get; init; }
    }
}