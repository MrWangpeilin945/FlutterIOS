namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities
{
    /// <summary>
    /// 外部連携キーエンティティ
    /// </summary>
    public class ExternalConnectionCodeEntity
    {
        /// <summary>
        /// 受診ID
        /// </summary>
        public required Guid ConsultId { get; init; }

        /// <summary>
        /// 外部連携キー
        /// </summary>
        public required string ConnectionCode { get; init; }
    }
}
