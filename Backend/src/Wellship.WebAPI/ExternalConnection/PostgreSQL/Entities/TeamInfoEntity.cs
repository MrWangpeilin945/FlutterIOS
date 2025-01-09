namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities
{
    /// <summary>
    /// 班（ID、コード）
    /// </summary>
    public class TeamInfoEntity
    {
        /// <summary>
        /// 班ID
        /// </summary>
        public required Guid TeamId { get; init; }
        /// <summary>
        /// 班コード
        /// </summary>
        public required string TeamCode { get; init; }
    }
}
