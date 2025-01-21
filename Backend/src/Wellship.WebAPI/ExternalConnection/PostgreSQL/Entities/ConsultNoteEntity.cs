namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 受診特記エンティティ
    /// </summary>
    public class ConsultNoteEntity
    {
        /// <summary>
        /// 検査特記コード
        /// </summary>
        public required string Code { get; init; }

        /// <summary>
        /// 特記事項
        /// </summary>
        public required string Note { get; init; }
    }
}
