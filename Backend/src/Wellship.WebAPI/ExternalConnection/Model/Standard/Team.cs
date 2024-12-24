namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 班
    /// </summary>
    public class Team
    {
        /// <summary>
        /// 班コード
        /// </summary>
        public required string Code { get; init; }

        /// <summary>
        /// 班名
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// エラーオブジェクトに登録する入力項目Noなど
        /// </summary>
        public required string InputNote { get; init; }
    }
}
