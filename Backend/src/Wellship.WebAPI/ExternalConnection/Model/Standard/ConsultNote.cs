using System.Text.Json.Serialization;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 受診特記.特記事項
    /// </summary>
    public class ConsultNote
    {
        /// <summary>
        /// 検査特記コード
        /// </summary>
        [JsonPropertyName("code")]
        public required string Code { get; init; }

        /// <summary>
        /// 特記事項
        /// </summary>
        [JsonPropertyName("note")]
        public required string Note { get; init; }
    }
}
