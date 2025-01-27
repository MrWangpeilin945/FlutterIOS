using System.Text.Json.Serialization;

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
        [JsonPropertyName("teamCode")]
        public required string TeamCode { get; init; }
        /// <summary>
        /// 会場コード
        /// </summary>
        [JsonPropertyName("placeCode")]
        public required string PlaceCode { get; init; }
        /// <summary>
        /// 健診日/開始時刻
        /// </summary>
        [JsonPropertyName("examDate")]
        public required DateTime ExamDate { get; init; }
        /// <summary>
        /// エラーオブジェクトに登録する入力項目Noなど
        /// </summary>
        [JsonPropertyName("inputNote")]
        public required string InputNote { get; init; }
    }
}
