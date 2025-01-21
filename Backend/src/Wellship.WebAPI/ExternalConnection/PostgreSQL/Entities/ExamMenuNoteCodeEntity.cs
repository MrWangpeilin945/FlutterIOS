namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities
{
    /// <summary>
    /// 検査メニュー特記コード
    /// </summary>
    public class ExamMenuNoteCodeEntity
    {
        /// <summary>
        /// 検査特記コード
        /// </summary>
        public required string Code { get; init; }
        /// <summary>
        /// 検査特記名
        /// </summary>
        public required string Name { get; init; }
    }
}
