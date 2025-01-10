namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 過去検査結果
    /// </summary>
    public class PreviousResult
    {
        /// <summary>
        /// 検査項目明細CD
        /// </summary>
        public required string ExamItemDetailCd { get; init; }

        /// <summary>
        /// 検査日
        /// </summary>
        public required DateOnly ExamDate { get; init; }

        /// <summary>
        /// 結果値
        /// </summary>
        public required string Value { get; init; }
    }
}