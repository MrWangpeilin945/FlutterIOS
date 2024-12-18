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
        /// 班名
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// 表示順
        /// </summary>
        public int OrderNumber { get; init; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public required DateTime CreatedAt { get; init; }

        /// <summary>
        /// 作成者
        /// </summary>
        public required string CreatedBy { get; init; }
    }
}
