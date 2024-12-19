namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities
{
    /// <summary>
    /// 団体エンティティ
    /// </summary>
    public class OrganizationEntity
    {
        /// <summary>
        /// 団体コード
        /// </summary>
        public required string OrganizationCode { get; init; }

        /// <summary>
        /// 団体名
        /// </summary>
        public required string Name { get; init; }
    }
}
