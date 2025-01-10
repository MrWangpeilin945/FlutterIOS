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
        /// 会場コード
        /// </summary>
        public required string PlaceCode { get; init; }

        /// <summary>
        /// 班コード
        /// </summary>
        public required string TeamCode { get; init; }

        /// <summary>
        /// 健診日
        /// </summary>
        public required DateOnly ExamDate { get; init; }

        /// <summary>
        /// 受診番号
        /// </summary>
        public required string ConsultNumber { get; init; }

        /// <summary>
        /// 受診者コード
        /// </summary>
        public required string ExamineeCd { get; init; }

        /// <summary>
        /// 受診.特記事項
        /// </summary>
        public required string Note { get; init; }

        /// <summary>
        /// 過去検査結果
        /// </summary>
        public required List<PreviousResult> PreviousResults { get; init; }

        /// <summary>
        /// 基準値判定
        /// </summary>
        public required List<ConsultThreshold> ConsultThresholds { get; init; }

        /// <summary>
        /// 受診特記.特記事項
        /// </summary>
        public required List<ConsultNote> ConsultNotes { get; init; }

        /// <summary>
        /// 検査項目明細依頼
        /// </summary>
        public required List<ExamItemDetailOrder> ExamItemDetailOrders { get; init; }

        /// <summary>
        /// エラーオブジェクトに登録する入力項目Noなど
        /// </summary>
        public required string InputNote { get; init; }
    }
}