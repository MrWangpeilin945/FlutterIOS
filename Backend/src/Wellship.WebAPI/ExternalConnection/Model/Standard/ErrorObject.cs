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
        public required string Code { get; init; }

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public required string Message { get; init; }

        /// <summary>
        /// 入力項目Noなど
        /// </summary>
        public required string InputNote { get; init; }
    }
}
