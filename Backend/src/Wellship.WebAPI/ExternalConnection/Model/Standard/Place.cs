namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 会場
    /// </summary>
    public class Place
    {
        /// <summary>
        /// 会場コード
        /// </summary>
        public required string Code { get; init; }

        /// <summary>
        /// 会場名
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// エラーオブジェクトに登録する入力項目Noなど
        /// </summary>
        public required string InputNote { get; init; }
    }
}
