using System.Text.Json.Serialization;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 会場
    /// </summary>
    public class Place
    {
        /// <summary>
        /// 会場コード
        /// </summary>
        [JsonPropertyName("code")]
        public required string Code { get; init; }

        /// <summary>
        /// 会場名
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; init; }

        /// <summary>
        /// エラーオブジェクトに登録する入力項目Noなど
        /// </summary>
        [JsonPropertyName("inputNote")]
        public required string InputNote { get; init; }
    }
}
