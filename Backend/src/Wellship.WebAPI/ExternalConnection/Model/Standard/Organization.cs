namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 団体
    /// </summary>
    public class Organization
    {
        /// <summary>
        /// 団体コード
        /// </summary>
        public required string Code { get; init; }

        /// <summary>
        /// 団体名
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// エラーオブジェクトに登録する入力項目Noなど
        /// </summary>
        public required string InputNote { get; init; }
    }
}
