namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 過去検査結果エンティティ
    /// </summary>
    public class PreviousResultEntity
    {
        /// <summary>
        /// 健診日
        /// </summary>
        public required DateOnly ExamDate { get; init; }

        /// <summary>
        /// 検査項目明細ID
        /// </summary>
        public required int ExamItemDetailId { get; init; }

        /// <summary>
        /// 値
        /// </summary>
        public required string Value { get; init; }
    }
}
