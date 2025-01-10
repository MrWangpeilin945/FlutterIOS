namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities
{
    /// <summary>
    /// 会場日程エンティティ
    /// </summary>
    public class PlaceScheduleEntity
    {
        /// <summary>
        /// 会場ID
        /// </summary>
        public required Guid PlaceId { get; init; }

        /// <summary>
        /// 班ID
        /// </summary>
        public required Guid TeamId { get; init; }

        /// <summary>
        /// 状況
        /// </summary>
        public required int Status { get; init; }

        /// <summary>
        /// 健診日
        /// </summary>
        public required DateTime ExamDate { get; init; }

        /// <summary>
        /// 開始時刻
        /// </summary>
        public required string StartTime { get; init; }
    }
}
