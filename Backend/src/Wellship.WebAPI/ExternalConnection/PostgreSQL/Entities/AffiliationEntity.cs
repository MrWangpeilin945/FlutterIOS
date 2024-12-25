namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities
{
    /// <summary>
    /// 所属エンティティ
    /// </summary>
    public class AffiliationEntity
    {
        /// <summary>
        /// 受診者ID
        /// </summary>
        public required Guid ExamineeId { get; init; }

        /// <summary>
        /// 受診者コード
        /// </summary>
        public required string ExamineeCode { get; init; }

        /// <summary>
        /// 団体ID
        /// </summary>
        public required Guid OrganizationId { get; init; }

        /// <summary>
        /// 団体コード
        /// </summary>
        public required string OrganizationCode { get; init; }
    }
}
