using System.Text.Json.Serialization;
using Ryobi.Wellship.ExternalConnection.Enums;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 受付
    /// </summary>
    public class Ticket
    {
        /// <summary>
        /// 処理順
        /// </summary>
        [JsonPropertyName("sortNo")]
        public required int SortNo { get; init; }

        /// <summary>
        /// 連携キー
        /// </summary>
        [JsonPropertyName("connectionCode")]
        public required string ConnectionCode { get; init; }

        /// <summary>
        /// 連携モード
        /// </summary>
        [JsonPropertyName("actionType")]
        public required ActionType ActionType { get; init; }

        /// <summary>
        /// 受付番号
        /// </summary>
        [JsonPropertyName("ticketNumber")]
        public required string TicketNumber { get; init; }

        /// <summary>
        /// エラーオブジェクトに登録する入力項目Noなど
        /// </summary>
        [JsonPropertyName("inputNote")]
        public required string InputNote { get; init; }
    }
}
