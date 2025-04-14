using System.Text.Json.Serialization;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// エラーオブジェクト
    /// </summary>
    public class ErrorObject
    {
        /// <summary>
        /// エラーコード
        /// </summary>
        [JsonPropertyName("code")]
        public required string Code { get; init; }

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        [JsonPropertyName("message")] 
        public required string Message { get; init; }

        /// <summary>
        /// 入力項目Noなど
        /// </summary>
        [JsonPropertyName("inputNote")]
        public required string InputNote { get; init; }
    }
}
