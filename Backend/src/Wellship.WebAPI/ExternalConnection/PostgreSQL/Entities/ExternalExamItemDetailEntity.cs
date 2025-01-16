namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities
{
    /// <summary>
    /// 外部検査項目明細
    /// </summary>
    public class ExternalExamItemDetailEntity
    {
        /// <summary>
        /// 検査項目明細ID
        /// </summary>
        public required int ExamItemDetailId { get; init; }
        /// <summary>
        /// 外部コード検査項目明細CD
        /// </summary>
        public required string ExternalExamItemDetailCode { get; init; }
    }
}
