using Ryobi.Wellship.WebAPI.ExternalConnection.Enums;
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities
{
    /// <summary>
    /// 受診エンティティ
    /// </summary>
    public class ConsultEntity
    {
        /// <summary>
        /// 連携モード
        /// </summary>
        public required ActionType ActionType { get; set; }

        /// <summary>
        /// 受診ID
        /// </summary>
        public Guid ConsultId { get; init; }

        /// <summary>
        /// 受診番号
        /// </summary>
        public required string ConsultNumber { get; init; }

        /// <summary>
        /// 会場日程ID
        /// </summary>
        public required Guid PlaceScheduleId { get; init; }

        /// <summary>
        /// 特記事項
        /// </summary>
        public required string Note { get; init; }

        /// <summary>
        /// 受診者ID
        /// </summary>
        public required Guid ExamineeId { get; init; }

        /// <summary>
        /// 連携キー
        /// </summary>
        public required string ConnectionCode { get; init; }

        /// <summary>
        /// 処理順
        /// </summary>
        public required int SortNo { get; init; }

        /// <summary>
        /// 受診特記
        /// </summary>
        public required List<ConsultNoteEntity> ConsultNotes { get; init; }

        /// <summary>
        /// 検査項目明細依頼
        /// </summary>
        public required List<ExamItemDetailOrderEntity> ExamItemDetailOrders { get; init; }

        /// <summary>
        /// 基準値判定
        /// </summary>
        public required List<ConsultThresholdEntity> ConsultThresholds { get; init; }

        /// <summary>
        /// 過去検査結果
        /// </summary>
        public required List<PreviousResultEntity> PreviousResults { get; init; }

    }
}
