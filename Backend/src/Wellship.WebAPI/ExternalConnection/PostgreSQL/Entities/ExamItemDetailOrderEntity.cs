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
        public required string ExamItemDetailCd { get; init; }
    }
}
