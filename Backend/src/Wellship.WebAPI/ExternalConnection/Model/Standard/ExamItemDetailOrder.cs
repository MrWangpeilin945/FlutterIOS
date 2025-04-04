using System.Text.Json.Serialization;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 検査項目明細依頼
    /// </summary>
    public class ExamItemDetailOrder
    {
        /// <summary>
        /// 検査項目明細CD
        /// </summary>
        [JsonPropertyName("examItemDetailCode")]
        public required string ExamItemDetailCode { get; init; }

        /// <summary>
        /// 検査項目明細備考
        /// </summary>
        [JsonPropertyName("note")]
        public required string Note { get; init; }
    }
}
