using System.Text.Json.Serialization;

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
        [JsonPropertyName("examItemDetailCode")]
        public required string ExamItemDetailCode { get; init; }

        /// <summary>
        /// 検査日
        /// </summary>
        [JsonPropertyName("examDate")]
        public required DateOnly ExamDate { get; init; }

        /// <summary>
        /// 結果値
        /// </summary>
        [JsonPropertyName("value")]
        public required string Value { get; init; }
    }
}