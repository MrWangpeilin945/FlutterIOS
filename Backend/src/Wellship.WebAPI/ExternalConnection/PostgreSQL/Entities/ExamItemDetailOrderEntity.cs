namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 受診特記エンティティ
    /// </summary>
    public class ExamItemDetailOrderEntity
    {
        /// <summary>
        /// 検査項目明細ID
        /// </summary>
        public required int ExamItemDetailId { get; init; }

        /// <summary>
        /// 検査項目明細CD
        /// </summary>
        public required string ExamItemDetailCode { get; init; }

        /// <summary>
        /// 検査項目明細備考
        /// </summary>
        public required string Note { get; init; }
    }
}
