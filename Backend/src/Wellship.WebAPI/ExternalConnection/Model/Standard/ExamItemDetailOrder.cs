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
        [JsonPropertyName("examItemDetailCd")]
        public required string ExamItemDetailCd { get; init; }
    }
}
