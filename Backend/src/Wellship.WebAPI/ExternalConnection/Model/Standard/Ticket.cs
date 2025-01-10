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
        public required int SortNo { get; init; }

        /// <summary>
        /// 連携キー
        /// </summary>
        public required string ConnectionCode { get; init; }

        /// <summary>
        /// 連携モード
        /// </summary>
        public required ActionType ActionType { get; init; }

        /// <summary>
        /// 受付番号
        /// </summary>
        public string TicketNumber { get; init; } = "";

        /// <summary>
        /// エラーオブジェクトに登録する入力項目Noなど
        /// </summary>
        public required string InputNote { get; init; }
    }
}
