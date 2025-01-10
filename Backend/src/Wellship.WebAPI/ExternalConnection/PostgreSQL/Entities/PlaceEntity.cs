namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities
{
    /// <summary>
    /// 会場
    /// </summary>
    public class PlaceEntity
    {
        /// <summary>
        /// 会場ID
        /// </summary>
        public required Guid PlaceId { get; init; }
        /// <summary>
        /// 会場コード
        /// </summary>
        public required string PlaceCode { get; init; }

        /// <summary>
        /// 会場名
        /// </summary>
        public required string Name { get; init; }
    }
}
