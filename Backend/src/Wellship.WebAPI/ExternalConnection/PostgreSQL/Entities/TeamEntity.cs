namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities
{
    /// <summary>
    /// 班
    /// </summary>
    public class TeamEntity
    {
        /// <summary>
        /// 班コード
        /// </summary>
        public required string TeamCode { get; init; }

        /// <summary>
        /// 班名
        /// </summary>
        public required string Name { get; init; }
    }
}
