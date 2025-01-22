using System.Text.Json.Serialization;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 班
    /// </summary>
    public class Team
    {
        /// <summary>
        /// 班コード
        /// </summary>
        [JsonPropertyName("code")]
        public required string Code { get; init; }

        /// <summary>
        /// 班名
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
