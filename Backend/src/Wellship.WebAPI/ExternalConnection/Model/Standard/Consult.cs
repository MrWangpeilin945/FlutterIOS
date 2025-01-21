using System.Text.Json.Serialization;
using Ryobi.Wellship.ExternalConnection.Enums;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 登録する受診のリスト
    /// </summary>
    public class Consult
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
        /// 会場コード
        /// </summary>
        [JsonPropertyName("placeCode")]
        public required string PlaceCode { get; init; }

        /// <summary>
        /// 班コード
        /// </summary>
        [JsonPropertyName("teamCode")]
        public required string TeamCode { get; init; }

        /// <summary>
        /// 健診日
        /// </summary>
        [JsonPropertyName("examDate")]
        public required DateOnly ExamDate { get; init; }

        /// <summary>
        /// 受診番号
        /// </summary>
        [JsonPropertyName("consultNumber")]
        public required string ConsultNumber { get; init; }

        /// <summary>
        /// 受診者コード
        /// </summary>
        [JsonPropertyName("examineeCd")]
        public required string ExamineeCd { get; init; }

        /// <summary>
        /// 受診.特記事項
        /// </summary>
        [JsonPropertyName("note")]
        public required string Note { get; init; }

        /// <summary>
        /// 過去検査結果
        /// </summary>
        [JsonPropertyName("previousResults")]
        public required List<PreviousResult> PreviousResults { get; init; }

        /// <summary>
        /// 基準値判定
        /// </summary>
        [JsonPropertyName("consultThresholds")]
        public required List<ConsultThreshold> ConsultThresholds { get; init; }

        /// <summary>
        /// 受診特記.特記事項
        /// </summary>
        [JsonPropertyName("consultNotes")]
        public required List<ConsultNote> ConsultNotes { get; init; }

        /// <summary>
        /// 検査項目明細依頼
        /// </summary>
        [JsonPropertyName("examItemDetailOrders")]
        public required List<ExamItemDetailOrder> ExamItemDetailOrders { get; init; }

        /// <summary>
        /// エラーオブジェクトに登録する入力項目Noなど
        /// </summary>
        [JsonPropertyName("inputNote")]
        public required string InputNote { get; init; }
    }
}