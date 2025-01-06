namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities
{
    /// <summary>
    /// 受診者エンティティ
    /// </summary>
    public class ExamineeEntity
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
        /// 氏名
        /// </summary>
        public required string Name {  get; init; }

        /// <summary>
        /// カナ氏名
        /// </summary>
        public required string KanaName { get; init; }

        /// <summary>
        /// 性別
        /// </summary>
        public required int Sex { get; init; }

        /// <summary>
        /// 生年月日
        /// </summary>
        public required DateTime Birthdate { get; init; }
    }
}
