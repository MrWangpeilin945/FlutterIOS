namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 基準パターン
    /// </summary>
    public class Threshold
    {
        /// <summary>
        /// 基準パターンコード
        /// </summary>
        public required string Code { get; init; }

        /// <summary>
        /// 基準パターン名
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// エラーオブジェクトに登録する入力項目Noなど
        /// </summary>
        public required string InputNote { get; init; }
    }
}
