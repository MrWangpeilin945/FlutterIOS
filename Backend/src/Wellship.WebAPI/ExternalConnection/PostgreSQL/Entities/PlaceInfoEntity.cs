namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities
{
    /// <summary>
    /// 会場（ID、コード）
    /// </summary>
    public class PlaceInfoEntity
    {
        /// <summary>
        /// 会場ID
        /// </summary>
        public required Guid PlaceId { get; init; }
        /// <summary>
        /// 会場コード
        /// </summary>
        public required string PlaceCode { get; init; }
    }
}
