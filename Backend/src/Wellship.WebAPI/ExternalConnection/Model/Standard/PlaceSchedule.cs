namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 会場日程
    /// </summary>
    public class PlaceSchedule
    {
        /// <summary>
        /// 班コード
        /// </summary>
        public required string TeamCode { get; init; }
        /// <summary>
        /// 会場コード
        /// </summary>
        public required string PlaceCode { get; init; }
        /// <summary>
        /// 健診日/開始時刻
        /// </summary>
        public required DateTime ExamDate { get; init; }
        /// <summary>
        /// エラーオブジェクトに登録する入力項目Noなど
        /// </summary>
        public required string InputNote { get; init; }
    }
}
